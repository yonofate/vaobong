/**
* jQuery methods for handling attachments.
*
* 2012-04-10   @tidyui
*/

if (!piranha)
    var piranha = {};

piranha.folderId = null;
piranha.media = function (buttonId, floatboxId, callback) {
    var self = this;

    this.btnId = buttonId;
    this.boxId = floatboxId;
    this.cb = callback;
    this.tinymce = false;
    this.filter = 'none';

    this.width = 0;
    this.height = 0;

    this.buttonClick = function () {
        $('#' + self.boxId + ' .box > div').remove();
        floatBox.show(self.boxId);
        $.ajax({
            url: siteroot + 'manager/content/popup' + (piranha.folderId ? '/' + piranha.folderId : '') + '?tinymce=' + self.tinymce + '&filter=' + self.filter,
            success: function (data) {
                $('#' + self.boxId + ' .box').html('');
                $('#' + self.boxId + ' .box').append(data);
                floatBox.position($('#' + self.boxId + ' .box'));
                self.storeDimensions();
            }
        });
    };
    this.mediaClick = function () {
        var that = this;

        if (!$(this).hasClass("folder")) {
            $('#' + self.boxId + ' .media-existing .loader').fadeIn('fast', function () {
                $.ajax({
                    url: siteroot + "manager/content/get/" + $(that).attr("data-id") + '?tinymce=' + self.tinymce,
                    dataType: "json",
                    success: function (data) {
                        if (self.cb)
                            self.cb(data);
                        floatBox.close(self.boxId);
                    }
                });
            });
        }
    };
    this.folderClick = function () {
        var that = this;

        piranha.folderId = $(this).attr("data-id");

        $('#' + self.boxId + ' .media-existing .loader').fadeIn('fast', function () {
            $.ajax({
                url: siteroot + "manager/content/popup/" + $(that).attr("data-id") + '?tinymce=' + self.tinymce + '&filter=' + self.filter,
                success: function (data) {
                    $('#' + self.boxId + ' .box').html('');
                    $('#' + self.boxId + ' .box').append(data);
                    floatBox.position($('#' + self.boxId + ' .box'));
                    self.storeDimensions();
                }
            });
        });
    };
    this.boxClose = function () {
        floatBox.close(self.boxId);
    };
    this.storeDimensions = function () {
        var mbox = $('#' + self.boxId + ' .media-existing');
        self.width = mbox.width();
        self.height = mbox.height();
    };
    this.boxDimension = function () {
        $('#' + self.boxId + ' .media-new').css({
            minWidth: self.width,
            minHeight: self.height,
            maxWidth: 'none'
        });
        floatBox.position($('#' + self.boxId + ' .box'));
    };
    this.upload = function () {
        var file = $('#' + self.boxId + ' .media-new-file').get(0);

        $('#' + self.boxId + ' .media-new .loader').fadeIn('fast', function () {
            $.ajax({
                url: siteroot + 'manager/content/upload',
                type: 'POST',
                contentType: 'multipart/form-data',
                dataType: 'json',
                processData: false,
                headers: {
                    'X-File-Name': file.files[0].name,
                    'X-File-Type': file.files[0].type,
                    'X-File-Size': file.files[0].size,
                    'X-File-DisplayName': $('#' + self.boxId + ' .new-name').val(),
                    'X-File-Alt': $('#' + self.boxId + ' .new-alt').val(),
                    'X-File-Desc': $('#' + self.boxId + ' .new-desc').val(),
                    'X-File-ParentId': $('#' + self.boxId + ' .new-parentid').val()
                },
                data: file.files[0],
                success: function (data) {
                    if (self.cb)
                        self.cb(data);
                    floatBox.close(self.boxId);
                }
            });
        });
        return false;
    };

    $('#' + this.boxId + ' .gallery-item img').die('click').live('click', this.mediaClick);
    $('#' + this.boxId + ' .gallery-item img.folder').die('click').live('click', this.folderClick);
    $('#' + this.btnId).die('click').live('click', this.buttonClick);
    $('#' + this.boxId + ' .title .close-btn').die('click').live('click', this.boxClose);
    $('#' + this.boxId + ' .box-tabs .btn-media-new').die('click').live('click', this.boxDimension);
    $('#' + this.boxId + ' .media-new-upload').die('click').live('click', this.upload);
};
