function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode != 46) && (charCode != 44))
        return false;
    return true;
}

function minmax(value, min, max) {
    if (parseInt(value) < min || isNaN(value))
        return min;
    else if (parseInt(value) > max)
        return max;
    else return value;
}

function isIntegerKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function isHourKey(pInput) {
    if (Number($(pInput).val()) > 23) {
        $(pInput).val(23);
    }
}

function isMinuteKey(pInput) {
    if (Number($(pInput).val()) > 59) {
        $(pInput).val(59);
    }
}

function clickCancel(e) {
    if (!e) e = window.event;
    e.cancelBubble = true;
    if (e.stopPropagation) e.stopPropagation();
    e.preventDefault();

    return false;
}

function createCookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = escape(name) + "=" + escape(value) + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = escape(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return unescape(c.substring(nameEQ.length, c.length));
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}
var latin_map = {
    'ă': 'a', 
    'ș': 's',
    'ț': 's',
    'î': 'i'
};

function latinize(s) {
    return s.replace(/[^A-Za-z0-9_]/g, function (x) { return latin_map[x] || x; });
}

var latin_map_decode =  new Array( 
    new Array('&#197;', 'Å'),
    new Array('&#198;', 'ă'),
    new Array('&#258;', 'Ă'),
    new Array('&#259;', 'ă'),
    new Array('&#194;', 'Â'),
    new Array('&#226;', 'â'),
    new Array('&#206;', 'Î'),
    new Array('&#238;', 'î'),
    new Array('&#x218;', 'Ș'),
    new Array('&#x219;', 'ș'),
    new Array('&#350;', 'Ş'),
    new Array('&#351;', 'ş'),
    new Array('&#538;', 'Ț'),
    new Array('&#539;', 'ț'),
    new Array('&#354;', 'Ţ'),
    new Array('&#355;', 'ţ'),
    new Array('&quot;', '\"'),
    new Array('&gt;', '>'),
    new Array('&lt;', '<')
)

function latinizeDecode(s) {
    for (var i = 0; i < latin_map_decode.length; i++) {
        while (s.indexOf(latin_map_decode[i][0]) > -1) {
            s = s.replace(latin_map_decode[i][0], latin_map_decode[i][1]);
        }
    }
    return s;
}

function generateTag(s) {
    s = s.replace(new RegExp(" ", 'g'), "_");
    s = latinize(s.toLowerCase());
    return s;
}

function StrToDate(str) {
    var parts = str.split('/');
    //please put attention to the month (parts[0]), Javascript counts months from 0:
    // January - 0, February - 1, etc
    return new Date(parts[2], parts[1] - 1, parts[0]);
}