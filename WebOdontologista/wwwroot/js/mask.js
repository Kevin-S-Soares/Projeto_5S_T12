﻿function mask(text) {
    if (text.value.length == 10) {
        if (text.value[4] != " ") {
            var initialDigits = "(" + text.value.substring(0, 2) + ") ";
            var middleDigits = text.value.substring(2, 6) + "-";
            var finalDigits = text.value.substring(6, 10);
            text.value = initialDigits + middleDigits + finalDigits;
        }
    }
    else if (text.value.length == 13) {
        if (text.value[9] != "-") {
            var initialDigits = text.value.substring(0, 9);
            var finalDigits = text.value.substring(9, 13);
            text.value = initialDigits + "-" + finalDigits;
        }
    }
    else if (text.value.length == 14) {
        if (text.value[9] != "-") {
            var initialDigits = text.value.substring(0, 9);
            var finalDigits = text.value.substring(11, 14);
            text.value = initialDigits + "-" + text.value[9] + finalDigits;
        }
    }
    else if (text.value.length == 15) {
        if (text.value[10] != "-") {
            var initialDigits = text.value.substring(0, 9);
            var finalDigits = text.value.substring(11, 15);
            text.value = initialDigits + text.value[10] + "-" + finalDigits;
        }
    }
}