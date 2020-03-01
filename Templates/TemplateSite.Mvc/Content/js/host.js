function toMoney(m) {
    if (m == undefined)
        return 0;

    if (typeof (m) != 'number')
        m = parseInt(m);

    var s = m.toFixed(1).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    return s.substr(0, s.length - 2);
}

function HostViewModel() {
    var hosts = [
        { logo: 'logo-fun88.png', h: 'fun88', pro_r: 150, pro_p: 5000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 9, count: 540099, site: 'http://www.fun183.com/vi-VN/Home' },
        { logo: 'logo-w88.png', h: 'w88', pro_r: 100, pro_p: 4000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 9.5, count: 878273, site: 'https://www.w88wasia.com/_secure/register.aspx?' },
        { logo: 'logo-m88.png', h: 'm88', pro_r: 33, pro_p: 5888000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 9.5, count: 540099, site: 'https://www.m998877.com/Main/Home.aspx?lang=vi-VN' },
        { logo: 'logo-12bet.png', h: '12bet', pro_r: 35, pro_p: 6688000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 9, count: 60654, site: 'http://www.247viet.com/index.aspx?lang=vn' },
        { logo: 'logo-188bet.png', h: '188bet', pro_r: 28, pro_p: 6000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'dễ sử dụng', rate: 8, count: 587890, site: 'https://www.512jbb.com/vi-vn/' },
        { logo: 'logo-happyluke.png', h: 'happy like', pro_r: 100, pro_p: 10000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 8.5, count: 556255, site: 'https://www.hlv88.com/#join', sm: true },
        { logo: 'logo-dafabet.png', h: 'dafabet', pro_r: 100, pro_p: 1200000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 516127, site: 'http://sportsbook.dafatiyu.net/vn?btag=628778_98FB08D9456E48688DD65B37976B083C' },
        { logo: 'logo-vwin.png', h: 'vwin', pro_r: 38, pro_p: 8888000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 8.5, count: 589007, site: 'http://www.vw606.com/#/affiliate/agent?id=12' },
        { logo: 'logo-empire777.png', h: 'empire', pro_r: 100, pro_p: 2000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 124534, site: 'https://www.empire777vn.com/vi', hm: true },
        { logo: 'logo-138bet.png', h: '138bet', pro_r: 30, pro_p: 5000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 526079, site: 'http://www.138gs.biz/vi-vn' },
        { logo: 'logo-v9bet.png', h: 'v9bet', pro_r: 100, pro_p: 2000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 290045, site: 'http://www.v9bet.com/vi-vn/?affcode=ptt123' },
        { logo: 'logo-happy8.png', h: 'happy8', pro_r: 20, pro_p: 4000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 540099, site: 'https://www.happy8vnd.com/Main/Register.aspx?affiliateId=86219' },
        { logo: 'logo-cmd368.png', h: 'cmd368', pro_r: 50, pro_p: 20000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 323459, site: 'http://www.9314504.com/mem/promotion.aspx?v=F384306F94951868DE61E90A78EA67ABF71D4515C097662E&affl=vi-VN' },
        { logo: 'logo-vegas.png', h: 'vegas', pro_r: 30, pro_p: 7900000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 300156, site: 'http://vegas79.com/?refcode=KQN' },
        { logo: 'logo-boma365.jpg', h: 'boma', pro_r: 30, pro_p: 3800000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7.5, count: 145621, site: 'http://www.boma365vn.com/079212/' },
        { logo: 'logo-365bet.png', h: 'bet365', pro_r: 100, pro_p: 5000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 6.5, count: 289145, site: 'http://www.game-365.com/en/?script=mainpage.asp' },
        { logo: 'logo-10bet.png', h: '10bet', pro_r: 50, pro_p: 7000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 6.5, count: 250004, site: 'http://vi.10bet.com/' },
        { logo: 'logo-dubai-casino.png', h: 'dubai', pro_r: 100, pro_p: 8888000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 6, count: 189006, site: 'http://www.322722.com/main/ot/agent/100282' },
        { logo: 'logo-verajohn.png', h: 'vera & join', pro_r: 100, pro_p: 10000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 6, count: 168176, site: 'https://www.verajohn.com/', sm: true },
        { logo: 'logo-g7bet.png', h: 'g7bet', pro_r: 28, pro_p: 28000000, deposit: 'nhanh', withdraw: 'nhanh', promo: 'nhiều', support: 'nhanh', odds: 'cao', ui: 'đẹp', rate: 7, count: 301025, site: 'http://www.hugedomains.com/domain_profile.cfm?d=g7bet&e=com' },
    ];

    var self = this;

    self.showButton = ko.observable(true);
    self.hosts = ko.observableArray([]);
    for (var i = 0; i < 10; i++) {
        self.hosts.push(hosts[i]);
    }
    self.loadMoreHost = function () {
        for (var i = 10; i < hosts.length; i++) {
            self.hosts.push(hosts[i]);
        }
        self.showButton(false);
    }

}

ko.applyBindings(new HostViewModel());