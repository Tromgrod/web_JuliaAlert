using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Helpers;
using LIB.Tools.BO;
using LIB.Tools.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.Packaging.Ionic.Zip;
using Weblib.Exceptions;
using Weblib.Helpers;
using Weblib.Models.Api;

namespace Weblib.Controllers
{
    public class ApiDataProcessor : BaseApiController
    {
        private readonly string _namespace = "LIB.BusinessObjects";

        /// <summary>
        ///     Init API data processor.
        /// </summary>
        /// <param name="ns">Default namespace</param>
        public ApiDataProcessor(string ns)
        {
            _namespace = ns;
        }

        #region GET
        /// <summary>
        ///     Gets filtered object or objects by type and filter.
        ///
        ///     Besides object properties you can use filter parameters.
        ///     Such as:
        ///         startIndex - start array from index
        ///         limit - array limit
        ///         order - array order. Default is ascending. Can be asc (ascending), desc (descending).
        ///         orderBy - order by property. Default is Id.
        /// 
        ///     Example: https://tempuri.org/Type/LIB.BusinessObjects?Id=1&Name=Test&...
        ///     Example: https://tempuri.org/Type?Id=1&Name=Test&...
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("{type}/{ns?}")]
        public HttpResponseMessage Get(string type, string ns = "")
        {
            var response = new ApiGetResponseModel
            {
                StartIndex = 0,
                Limit = 0,
                Total = 0,
                Order = "asc",
                OrderBy = "Id"
            };

            try
            {
                if (string.IsNullOrEmpty(ns)) ns = _namespace;

                var parameters = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, y => y.Value);

                if (parameters.Any(x => x.Key.Equals("startIndex"))
                    && int.TryParse(parameters["startIndex"], out var responseStartIndex))
                {
                    response.StartIndex = responseStartIndex;
                }

                if (parameters.Any(x => x.Key.Equals("limit"))
                    && int.TryParse(parameters["limit"], out var responseLimit))
                {
                    response.Limit = responseLimit;
                }

                var item = Activator.CreateInstance(Type.GetType(ns + "." + type + ", " + ns.Split('.')[0], true)) as ItemBase;

                if (item == null)
                    throw new TypeLoadException("The type is not derived form ItemBase.");

                var properties = item.GetType().GetProperties();
                
                foreach (var param in parameters)
                {
                    if (param.Value == null) continue;

                    var propr = properties.FirstOrDefault(x => x.Name == param.Key);
                    if (propr != null)
                    {
                        propr.SetValue(item, Convert.ChangeType(param.Value, propr.PropertyType));
                    }
                }

                IEnumerable<ItemBase> query = ((ItemBase)item).Populate((ItemBase)item).Values;

                if (parameters.Any(x => x.Key.Equals("order")))
                {
                    var isAsc = parameters["order"].Equals("asc", StringComparison.InvariantCultureIgnoreCase) ||
                                parameters["order"].Equals("ascending", StringComparison.InvariantCultureIgnoreCase);
                    PropertyInfo pi = null;
                    if (parameters.Any(x => x.Key.Equals("orderBy")))
                    {
                        var propOrder =
                            parameters.FirstOrDefault(x => x.Key.Equals("orderBy")).Value;

                        response.OrderBy = propOrder ?? response.OrderBy;

                        pi = properties.FirstOrDefault(x =>
                            x.Name.Equals(response.OrderBy, StringComparison.InvariantCultureIgnoreCase));
                    }
                    if (pi == null)
                        pi = properties.First(x => x.Name == response.OrderBy);

                    if (!isAsc)
                    {
                        query = query.OrderByDescending(x => pi.GetValue(x, null));
                        response.Order = "asc";
                    }
                    else
                    {
                        query = query.OrderBy(x => pi.GetValue(x, null));
                        response.Order = "desc";
                    }
                }

                response.Total = query.Count();

                if (response.StartIndex > 0 || response.Limit > 0)
                {
                    if (response.StartIndex > 0)
                        query = query.Skip(response.StartIndex);
                    if (response.Limit > 0)
                        query = query.Take(response.Limit);
                }
                IEnumerable<ItemBase> items = query.ToList();

                response.Data = items;
                response.Status = ApiResponseModel.ApiStatus.Success;
                return CreateResponse(response);
            }
            catch (TypeLoadException e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region POST
        /// <summary>
        ///     Inserts or updates item.
        ///     
        ///     Example: https://tempuri.org/Type
        ///     Body: { "Name": "test", "Email": "test@testing.com"}
        ///     Body: { "Id": 1, "Name": "test", "Email": "test@testing.com"}
        /// </summary>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("{type}")]
        public HttpResponseMessage Post(string type, [FromBody]JObject body)
        {
            var response = new ApiResponseModel
            {
                Status = ApiResponseModel.ApiStatus.Success
            };
            try
            {
                var item = Activator.CreateInstance(
                    Type.GetType(_namespace + "." + type + ", " + _namespace.Split('.')[0], true)) as ItemBase;
                if (item == null)
                    throw new BadRequestException("Cannot convert to ItemBase");

                if (body["Id"] != null)
                {
                    item.Id = Convert.ToInt64(body["Id"]);
                    item = item.PopulateOne(item);

                    if (item == null)
                        item = Activator.CreateInstance(
                            Type.GetType(_namespace + "." + type + ", " + _namespace.Split('.')[0], true)) as ItemBase;
                }

                var properties = item.GetType().GetProperties();
                foreach (JProperty xjp in (JToken) body)
                {
                    var prop = properties.FirstOrDefault(x =>
                        x.Name.Equals(xjp.Name, StringComparison.InvariantCultureIgnoreCase));

                    if (prop == null) continue;

                    if (prop.PropertyType.IsClass && !prop.PropertyType.IsValueType &&
                        prop.PropertyType != typeof(string))
                    {
                        var childItem = Activator.CreateInstance(prop.PropertyType) as ItemBase;
                        childItem.Id = Convert.ToInt64(xjp.Value.ToString());
                        childItem = childItem.PopulateOne(childItem);
                        prop.SetValue(item, childItem);
                        continue;
                    }

                    var value = Convert.ChangeType(xjp.Value.ToString(), prop.PropertyType);

                    if (xjp.Name == "Password")
                        value = ((User) item).GetpasswordHash();

                    prop.SetValue(item, value);
                }
                if (item.Id == 0)
                {
                    item.Insert(item);
                    response.Message = "Inserted";
                }
                else
                {
                    item.Update(item);
                    response.Message = "Updated";
                }
                response.Status = ApiResponseModel.ApiStatus.Success;
                response.Data = item;

                return CreateResponse(response);
            }
            catch (TypeLoadException e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.Conflict);
            }
            catch (BadRequestException e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.InternalServerError);
            }

        }
        #endregion

        #region PUT
        /// <summary>
        ///     Updates item.
        ///     
        ///     Example: https://tempuri.org/Type
        ///     Body: { "Id": 1, "Name": "test", "Email": "test@testing.com"}
        /// </summary>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("{type}")]
        public HttpResponseMessage Put(string type, [FromBody]JObject body)
        {
            var response = new ApiResponseModel
            {
                Status = ApiResponseModel.ApiStatus.Success
            };
            try
            {
                var item = Activator.CreateInstance(
                    Type.GetType(_namespace + "." + type + ", " + _namespace.Split('.')[0], true)) as ItemBase;
                if (item == null)
                    throw new BadRequestException("Cannot convert to ItemBase");

                if (body["Id"] == null)
                {
                    throw new BadRequestException("Update request has to contain Id property.");
                }
                item.Id = Convert.ToInt64(body["Id"]);
                item = item.PopulateOne(item);

                if (item == null)
                {
                    throw new NotFoundException("Item not found.");
                }

                var properties = item.GetType().GetProperties();
                foreach (JProperty xjp in (JToken)body)
                {
                    var prop = properties.FirstOrDefault(x => x.Name == xjp.Name);

                    if (prop == null) continue;

                    if (prop.PropertyType.IsClass && !prop.PropertyType.IsValueType && prop.PropertyType != typeof(string))
                    {
                        var childItem = Activator.CreateInstance(prop.PropertyType) as ItemBase;
                        childItem.Id = Convert.ToInt64(xjp.Value.ToString());
                        childItem = childItem.PopulateOne(childItem);
                        prop.SetValue(item, childItem);
                        continue;
                    }

                    var value = Convert.ChangeType(xjp.Value.ToString(), prop.PropertyType);

                    if (xjp.Name == "Password")
                        value = ((User)item).GetpasswordHash();

                    prop.SetValue(item, value);
                }

                item.Update(item);
                response.Data = item;
                response.Status = ApiResponseModel.ApiStatus.Success;
                response.Message = "Updated";
                return CreateResponse(response);
            }
            catch (TypeLoadException e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.Conflict);
            }
            catch (BadRequestException e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.BadRequest);
            }
            catch (NotFoundException e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        ///     Deletes item by id
        ///     
        ///     Example: https://tempuri.org/Type/1
        /// </summary>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("{type}/{id}")]
        public HttpResponseMessage Delete(string type, long id)
        {
            var response = new ApiResponseModel
            {
                Status = ApiResponseModel.ApiStatus.Success
            };
            try
            {
                var item = Activator.CreateInstance(Type.GetType(_namespace + "." + type + ", " + _namespace.Split('.')[0], true)) as ItemBase;
                item.Id = id;

                item.Delete(user: new User(1));
            }
            catch (TypeLoadException e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                response.Error = e.Message;
                response.Status = ApiResponseModel.ApiStatus.Error;
                return CreateResponse(response, HttpStatusCode.InternalServerError);
            }

            response.Status = ApiResponseModel.ApiStatus.Success;
            response.Message = "Deleted";
            return CreateResponse(response);
        }
        #endregion
    }
}
