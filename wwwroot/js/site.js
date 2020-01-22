function copyToClipboard(copiedText, el) {
    $('<textarea class="copied-text" contenteditable="true">' + copiedText + '</textarea>').appendTo('body');

    if (navigator.userAgent.match(/ipad|iphone/i)) {
        var range = document.createRange(),
            textArea = $('.copied-text')[0];
        range.selectNodeContents(textArea);
        var selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
        textArea.setSelectionRange(0, 999999);
    } else {
        $('.copied-text').select();
    }

    document.execCommand('copy');
    $('.copied-text').remove();

    var span = document.createElement("span");
    span.className = "hinttext";
    span.innerText = 'Copied to clipboard';
    el.appendChild(span);
    el.addEventListener("mouseout", hideHint);
}

function showPassword(id, el) {
    $.get("/Home/GetPassword/" + id, function (data) {
        $('<td class="hint" onclick="copyToClipboard(\'' + data + '\', this)">' + data + '</td>').replaceAll(el);
    });
}

function getPassword(id, el) {
    if (el.value !== '**********') {
        el.value = '**********';
    } else {
        $.get("/Home/GetPassword/" + id, function (data) {
            el.value = data;
        });
    }
}

function hideHint() {
    var hinttextspan = document.getElementsByClassName("hinttext")[0];
    hinttextspan.remove();
}

function filterTable(event) {
    var filter = (event === undefined) ? '' : event.target.value.toUpperCase();
    var rows = document.querySelector("#entries tbody").rows;

    for (var i = 0; i < rows.length; i++) {
        var groupCol = rows[i].cells[0].textContent.toUpperCase();
        var titleCol = rows[i].cells[1].textContent.toUpperCase();
        var usernameCol = rows[i].cells[2].textContent.toUpperCase();
        if (groupCol.indexOf(filter) > -1 || titleCol.indexOf(filter) > -1 || usernameCol.indexOf(filter) > -1) {
            rows[i].style.display = "";
        } else {
            rows[i].style.display = "none";
        }
    }
}

function GeneratePassword(length, fieldName) {
    var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%&*+-_=";
    var randomstring = '';
    for (var i = 0; i < length; i++) {
        var rnum = Math.floor(Math.random() * chars.length);
        randomstring += chars.substring(rnum, rnum + 1);
    }
    $('#' + fieldName).val(randomstring);
}

var searchEntries = document.querySelector('#searchEntries');
if (searchEntries !== null) searchEntries.addEventListener('keyup', filterTable, false);

function tog(v) { return v ? 'addClass' : 'removeClass'; }
$(document).on('input', '.clearable', function () {
    $(this)[tog(this.value)]('x');
}).on('mousemove', '.x', function (e) {
    $(this)[tog(this.offsetWidth - 18 < e.clientX - this.getBoundingClientRect().left)]('onX');
}).on('touchstart click', '.onX', function (ev) {
    ev.preventDefault();
    $(this).removeClass('x onX').val('').change();
    filterTable();
});


