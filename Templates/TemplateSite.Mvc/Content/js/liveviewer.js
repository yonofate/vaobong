function onLoadLiveViewer() {
    $.ajax({
        url: '/Page/LoadLiveViewer',
        method: 'get',
        success: function (resp) {
            console.log(resp);
        },
        error: function (jqXHR) {
            console.log(jqXHR);
        },
        complete: function () {
            console.log('completed');
        }
    });
}

function ViewerViewModel() {
    var self = this;

    self.liveviewer = ko.observable();
    
    self.selectedServer = ko.observable('');
    self.changeServer = function (server) {
        console.log('change to server', server);
        self.selectedServer(server);
    }
}

var viewerViewModel = new ViewerViewModel();