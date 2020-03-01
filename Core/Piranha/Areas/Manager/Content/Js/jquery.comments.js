/**
 * jQuery methods for handling comments.
 *
 * 2012-12-20   Håkan Edling    @tidyui
 */

if (!piranha)
    var piranha = {};

piranha.comment = function (parentId, notificationId, containerId) {
    var self = this;

    this.parentId = parentId;
    this.notifId = notificationId;
    this.contId = containerId;

    this.updateComments = function (data) {
        if (data.Success == 1) {
            var notification = $('#' + self.notifId);
            notification.html(data.New);
            if (data.New > 0)
                notification.show();
            else notification.hide();
            $.get(siteroot + 'manager/comment/list/' + self.parentId, function (data) {
                $('#' + self.contId).html(data);
            });
        }
    };
    this.approveClick = function () {
        var id = $(this).attr('data-id');
        $.get(siteroot + 'manager/comment/ajaxapprove/' + id, self.updateComments);
        return false;
    };
    this.rejectClick = function () {
        var id = $(this).attr('data-id');
        $.get(siteroot + 'manager/comment/ajaxreject/' + id, self.updateComments);
        return false;
    };
    this.deleteClick = function () {
        var id = $(this).attr('data-id');
        $.get(siteroot + 'manager/comment/ajaxdelete/' + id, self.updateComments);
        return false;
    };
    this.expandClick = function () {
        // Deselect everything
        if (!$(this).hasClass('active')) {
            $('#' + self.contId + ' .btn-comment-expand').removeClass('active');
            $('#' + self.contId + ' .comment-detail').hide();
        }
        // Toggle current
        var id = $(this).attr('data-id');
        $('#' + id).toggle();
        $(this).toggleClass('active');
        return false;
    };

    $('#' + this.contId + ' .btn-comment-expand').live('click', this.expandClick);
    $('#' + this.contId + ' .comment-approve').live('click', this.approveClick);
    $('#' + this.contId + ' .comment-reject').live('click', this.rejectClick);
    $('#' + this.contId + ' .comment-delete').live('click', this.deleteClick);
};
