using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using LIB.Tools.Utils;
using Nest;
using Nest.JsonNetSerializer;

namespace LIB.Connectors
{
    public class ElasticSearchConnector
    {
        private static ElasticClient _elasticClient;

        private ElasticSearchConnector()
        { }

        public static ElasticClient Client
        {
            get
            {
                if (_elasticClient != null) return _elasticClient;

                var nodes = new[]
                {
                    new Uri(Config.GetConfigValue("ElasticsearchUrl"))
                };

                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool, JsonNetSerializer.Default).DisableDirectStreaming();
                _elasticClient = new ElasticClient(connectionSettings);

                return _elasticClient;
            }
        }
    }
}
