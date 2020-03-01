'use strict';

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function StreamViewModel() {
    var self = this;

    self.champions = ko.observableArray([]);

    // determine server loading
    self.loadingData = ko.observable(false);
    self.formattedHref = function (url) {
        // console.log('url',url);
        var a = document.createElement('a');
        a.href = url;
        var path = a.pathname.replace('/', '').replaceAll('/', '_');
        //console.log('path', path);
        return '/tran-dau/' + path;
    }.bind(streamViewModel);
    // tabs
    
    self.tabs = [{ url: 'online', text: 'Trực tuyến' }, { url: 'today', text: 'Hôm nay' }, { url: 'all', text: 'Tất cả' }];
    self.chosenTabData = ko.observable();
    self.chosenTabId = ko.observable();

    // select tab func
    self.goToTab = function (tab) {
        console.log('go to tab', tab.url);
        self.chosenTabId(tab);

        onLoadLiveStreamLink(tab.url);
    };
    

    // find sopcast links
    /*
    self.findSopcastLink = function (match, event) {
        // event.preventDefault();
        var status = match.isShowLink();
        //console.log(match, match.url(), status);
        match.isShowLink(!status); // toggle div

        match.loadingLink(false);

        if (status) return; // close links

        match.loadingLink(true);
        match.links([]);

        // get data
        $.ajax({
            url: '/Page/FindSopcastLink',
            method: 'post',
            data: { link: match.url() },
            success: function (resp) {
                // console.log('sopcast link result',resp);
                if (!resp || resp.length === 0) return;
                var length = resp.length;

                for (var i = 0; i < length; i++) {
                    var link = resp[i],
                        sop = new SopcastLink(link.Url, link.Lang, link.Bitrate);
                    // console.log('sop', sop.url(), sop.lang(), sop.bitrate());
                    match.links.push(sop);
                }

                // console.log('total links sopcast', match.links.length);
            },
            error: function (jqXHR) {
                console.log(jqXHR);
            },
            complete: function () {
                match.loadingLink(false);
            }
        });
    }
    */
}

var streamViewModel = new StreamViewModel();

function onAddLiveChampion(resp) {

    // console.log('current champ',streamViewModel.champions().length);

    // console.log(resp);
    if (!resp || resp.length == 0) {
        // console.log('data not found');
        return;
    }

    var count = resp.length;
    // console.log('found %s champion', count);
    for (var i = 0; i < count; i++) {
        var champSource = resp[i],
            champ = new Champion(champSource.FlagClass, champSource.Champion),
            matchArr = champSource.Matches;

        //console.log(champ.champ());

        if (matchArr && matchArr.length > 0) {
            for (var j = 0; j < matchArr.length; j++) {
                var mSource = matchArr[j],
                    match = new Match(mSource.Id, mSource.ElapsedTime, mSource.Date, mSource.Team1, mSource.Team2, mSource.Score, mSource.IsPlaying, mSource.Url);

                // console.log('> id: %s %s', match.id(), match.isPlaying() ? 'playing' : '', match.time());

                champ.matches().push(match);
            }
        }

        streamViewModel.champions.push(champ);
    }

    // console.log('populate data completed -> %s champions found', streamViewModel.champions().length);
}

function onLoadLiveStreamLink(tab) {
    streamViewModel.loadingData(true);
    tab = typeof (tab) != 'undefined' ? tab : '';
    // clear old data
    console.log('load link on tab', tab);
    streamViewModel.champions([]);
    $.ajax({
        //url: '/Page/LoadLiveStreamLink?tab=' + tab,
        url: '/Page/LoadLiveLink?live=live&tab=' + tab,
        method: 'get',
        success: onAddLiveChampion,
        error: function (jqXHR, stt, err) {
            console.log(jqXHR, stt, err);
        },
        complete: function () {
            setTimeout(function () {
                streamViewModel.loadingData(false);
            }, 250);
        }
    });
}