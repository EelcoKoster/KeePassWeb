function copyToClipboard(value, el) {
    var textArea = document.createElement("textarea");
    textArea.value = value;
    var containerPos = document.getElementsByClassName("container")[0];
    containerPos.appendChild(textArea);
    textArea.select();

    try {
        var successful = document.execCommand('copy');
        var msg = successful ? 'successful' : 'unsuccessful';
        console.log('Copying text command was ' + msg);
    } catch (err) {
        console.log('Oops, unable to copy');
    }

    containerPos.removeChild(textArea);

    var span = document.createElement("span");
    span.className = "hinttext";
    span.innerText = 'Copied to clipboard';
    el.appendChild(span);
    el.addEventListener("mouseout", hideHint);
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

function GeneratePassword(length, fieldName) {
    var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%&*+-_=";
    var randomstring = '';
    for (var i = 0; i < length; i++) {
        var rnum = Math.floor(Math.random() * chars.length);
        randomstring += chars.substring(rnum, rnum + 1);
    }
    $('#' + fieldName).val(randomstring);
}
