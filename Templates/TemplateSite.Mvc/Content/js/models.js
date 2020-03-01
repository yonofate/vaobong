function Match(id, time, date, team1, team2, score, isPlaying, url, links) {
    this.isPlaying = ko.observable(isPlaying);
    this.id = ko.observable(id);
    this.time = ko.observable(time);
    this.date = ko.observable(date);
    this.team1 = ko.observable(team1);
    this.team2 = ko.observable(team2);
    this.score = ko.observable(score);
    this.url = ko.observable(url);
    links = typeof (links) !== 'undefined' ? links : [];
    this.links = ko.observableArray(links);
    this.isShowLink = ko.observable(false);
    this.loadingLink = ko.observable(false);
}

function SopcastLink(url, lang, bitrate) {
    this.url = ko.observable(url);
    lang = lang == '' ? 'n/a' : lang;
    bitrate = bitrate == '' ? 'n/a' : bitrate;
    this.lang = ko.observable(lang);
    this.bitrate = ko.observable(bitrate);
}

function Champion(flag, champ, matches) {
    this.flag = ko.observable(flag);
    this.champ = ko.observable(champ);
    matches = typeof (matches) !== 'undefined' ? matches : [];
    this.matches = ko.observableArray(matches);
}

function LiveViewer(team, time, server) {
    this.team = ko.observable(team);
    this.time = ko.observable(time);
    server = typeof (server) !== 'undefined' ? server : [];
    this.server = ko.observableArray(server);
}