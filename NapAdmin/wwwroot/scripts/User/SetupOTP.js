/* global NAP: false */

"use strict";

var SetupOTP = (function () {

    return {
        grid: null,
        form: null,
        siteId: null
    };

}(jQuery));

$(document).ready(function () {
    SetupOTP.grid = $('.grid');
    SetupOTP.form = $('form:first');
    SetupOTP.siteId = $('#SiteId');

    // shows loading on change page
    SetupOTP.form.submit(function () {
        NAP.loadingBox.show();
    });

    // Ignore OTP
    SetupOTP.grid.on('click', '.ignore-otp', function () {
        var $this = $(this);
        var custId = $this.data('cust-id');
        var system = $this.data('system');
        var secretKey = $("#secret-key-" + custId).data('secret-key');

        var ignoreOTP = $this.is(":checked");
        $this.prop('checked', !ignoreOTP);

        var msg = ignoreOTP
            ? `Are you sure you want to ignore OTP for this account ?`
            : `Are you sure you want to apply OTP for this account ?`
        var params = "?custId=" + custId
            + "&secretKey=" + secretKey
            + "&system=" + system
            + "&ignoreOTP=" + ignoreOTP;
        var url = NAP.resolveClientUrl("User/SetupOTPUpdate" + params);
        NAP.headlineMsgBox.confirm(msg, function () {
            $.ajax({
                url: url,
                type: 'POST',
                dataType: 'text',
                cache: false,
                beforeSend: function () {
                    NAP.loadingBox.show();
                },
                complete: function () {
                    NAP.loadingBox.close();
                },
                success: function (result) {
                    if (result.length === 0) {
                        var msgResult = ignoreOTP ? "OTP is ignored successfully!" : "OTP is applied successfully!";
                        NAP.headlineMsgBox.showInfo(msgResult, { closeBtn: false });
                        $this.prop('checked', ignoreOTP);
                    }
                },
                error: function (xhr, status, err) {
                    NAP.handleAjaxError(xhr, status, err);
                }
            });
        }, { modal: true, closeBtn: false });
    });

    SetupOTP.grid.on('click', '.ico-generate-normal', function () {
        var $this = $(this);
        var custId = $this.data('cust-id');
        var system = $this.data('system');
        var ignoreOTP = $("#chk-ignore-otp-" + custId).is(":checked");
        var params = "?custId=" + custId
            + "&system=" + system
            + "&ignoreOTP=" + ignoreOTP;
        var url = NAP.resolveClientUrl("User/SetupOTPGenerateKey" + params);

        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'json',
            cache: false,
            beforeSend: function () {
                NAP.loadingBox.show();
            },
            complete: function () {
                NAP.loadingBox.close();
            },
            success: function (result) {
                if (result.errMsg.length === 0) {
                    var msgResult = "Generate key successfully.";
                    $this.data("secret-key", result.secretKey)
                    $("#copy-key-" + custId).removeClass('ico-copy-disabled').addClass('ico-copy-normal');
                    NAP.headlineMsgBox.showInfo(msgResult, { closeBtn: false });
                }
                else {
                    NAP.headlineMsgBox.showErr(result.errMsg, { closeBtn: false });
                }
            },
            error: function (xhr, status, err) {
                NAP.handleAjaxError(xhr, status, err);
            }
        });
    });

    SetupOTP.grid.on('click', '.ico-copy-normal', function () {
        var $this = $(this);
        var custId = $this.data('cust-id');
        var secretKey = $("#secret-key-" + custId).data("secret-key");

        var $temp = $("<input>");
        $("body").append($temp);
        $temp.val(secretKey).select();
        document.execCommand("copy");
        $temp.remove();

        NAP.headlineMsgBox.showInfo("Copied OTP to clipboard.", { closeBtn: false });
    });
   
    // changes items/page
    SetupOTP.grid.on('change', '.items-per-page', function () {
        var $this = $(this),
            pageSize = $this.val(),
            username = $('#username').val(),
            system = $('#System').val(),
            status = $("#Status").val();
            
        var url = 'User/SetupOTP' +
            '?username=' + username +
            '&size=' + pageSize +
            '&system=' + system +
            '&status=' + status;
        url = NAP.resolveClientUrl(url);

        NAP.loadingBox.show();
        window.location.replace(url);
    });
});