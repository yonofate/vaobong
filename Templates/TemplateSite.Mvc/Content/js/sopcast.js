function SopcastViewModel() {
    var self = this;

    self.champions = ko.observableArray([]);

    // determine server loading
    self.loadingData = ko.observable(false);

    // tabs
    // self.tabs = [{ url: 'all', text: 'Tất cả' }, { url: 'today', text: 'Hôm nay' }, { url: 'online', text: 'Trực tuyến' }];
    self.tabs = [{ url: 'online', text: 'Trực tuyến' }, { url: 'today', text: 'Hôm nay' }, { url: 'all', text: 'Tất cả' }];
    self.chosenTabData = ko.observable();
    self.chosenTabId = ko.observable();    

    // select tab func
    self.goToTab = function(tab) {
        // console.log('go to tab', tab);
        self.chosenTabId(tab);

        onLoadSopcastLink(tab.url);
    };

    // find sopcast links
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
}

var sopcastViewModel = new SopcastViewModel();

function onAddSopcastChampion(resp) {

    // console.log('current champ',sopcastViewModel.champions().length);

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
        
        sopcastViewModel.champions.push(champ);
    }

    // console.log('populate data completed -> %s champions found', sopcastViewModel.champions().length);
}

function onLoadSopcastLink(tab) {
    sopcastViewModel.loadingData(true);
    tab = typeof (tab) != 'undefined' ? tab : '';
    // clear old data
    sopcastViewModel.champions([]);
    $.ajax({
        url: '/Page/LoadLiveLink?tab=' + tab,
        method: 'get',
        success: onAddSopcastChampion,
        error: function (jqXHR, stt, err) {
            console.log(jqXHR, stt, err);
        },
        complete: function () {
            setTimeout(function () {
                sopcastViewModel.loadingData(false);
            }, 250);
        }
    });
}

$(document).ready(function () {
    var $sopcast = $('#sopcast');
    if ($sopcast.length) {
        ko.applyBindings(sopcastViewModel);
        // sopcastViewModel.chosenTabId(sopcastViewModel.tabs[0]);

        sopcastViewModel.goToTab(sopcastViewModel.tabs[0]);
        // onLoadSopcastLink();


        setInterval(function () {
            if (sopcastViewModel.champions().length == 0) return;

            $.ajax({
                url: '/Page/UpdateLiveScore',
                method: 'get',
                success: function (resp) {
                    //console.log(resp);
                    if (!resp || resp.length == 0) return;

                    var len = resp.length,
                        champions = sopcastViewModel.champions(),
                        champCount = champions.length;

                    for (var i = 0; i < len; i++) {
                        var id = resp[i].Id,
                            time = resp[i].ElapsedTime,
                            score = resp[i].Score;

                        // loop thru champ to find match
                        for (var j = 0; j < champCount; j++) {
                            var matches = champions[j].matches(),
                                matchCount = matches.length;
                            for (var k = 0; k < matchCount; k++) {
                                if (matches[k].id() === id) {
                                    matches[k].time(time);
                                    matches[k].score(score);
                                    matches[k].isPlaying(true);
                                }
                            }
                        }
                    }
                },
                error: function (jqXHR) {
                    console.log(jqXHR);
                }
            });
        }, 3000);
    }
});