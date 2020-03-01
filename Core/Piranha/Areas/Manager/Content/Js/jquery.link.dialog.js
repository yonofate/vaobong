/**
* jQuery dialog for links.
*
* Håkan Edling      @tidyui
*/

if (!piranha)
    var piranha = {};

piranha.link = function (siteId, buttonId, floatboxId, callback) {
    var self = this;

    this.siteId = siteId;
    this.btnId = buttonId;
    this.boxId = floatboxId;
    this.cb = callback;

    this.buttonClick = function () {
        $('#' + self.boxId + ' .box > div').remove();
        floatBox.show(self.boxId);
        $.ajax({
            url: siteroot + 'manager/dialog/link/' + self.siteId,
            success: function (data) {
                $('#' + self.boxId + ' .box').html('');
                $('#' + self.boxId + ' .box').append(data);
                floatBox.position($('#' + self.boxId + ' .box'));
            }
        });
    };
    this.pageClick = function () {
        var that = this;

        $('#' + self.boxId + ' .link-page .loader').fadeIn('fast', function () {
            $.ajax({
                url: siteroot + "manager/page/get/" + $(that).attr("data-id"),
                dataType: "json",
                success: function (data) {
                    if (self.cb)
                        self.cb(data);
                    floatBox.close(self.boxId);
                }
            });
        });
    };
    this.boxClose = function () {
        floatBox.close(self.boxId);
    };

    $('#' + this.boxId + ' .link-page .sitemap a').die('click').live('click', this.pageClick);
    $('#' + this.btnId).die('click').live('click', this.buttonClick);
    $('#' + this.boxId + ' .title .close-btn').die('click').live('click', this.boxClose);
};