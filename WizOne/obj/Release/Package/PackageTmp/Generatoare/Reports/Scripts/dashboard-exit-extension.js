var CustomExtensions;
(function (CustomExtensions) {
    function ExitDashboardExtension(dashboardControl) {
        var _this = this;

        this.name = 'exit';
        this.dashboardControl = dashboardControl;
        this.toolboxExtension = this.dashboardControl.findExtension('toolbox');
        this.saveDashboardExtension = this.dashboardControl.findExtension('save-dashboard');
        this.template = 'dashboard-exit-extension';

        this._exitMenuItem = new DevExpress.Dashboard.DashboardMenuItem('exit', 'Exit', 120, 0, function () {            
            if (_this.saveDashboardExtension._isDashboardDirty()) {
                _this.savePopupMessage('"' + _this.dashboardControl.dashboard().title.text() + '" has been changed. Do you want to save changes ?');
                _this.savePopupVisible(true);
            } else {
                window.history.back();
            }
        });
        this._exitMenuItem.hasSeparator = true;
        this._exitMenuItem.data = _this;   
        this.savePopupVisible = ko.observable(false);
        this.savePopupMessage = ko.observable('');
        
        this.onSavePopupYesClick = function () {
            _this.savePopupVisible(false);
            _this.saveDashboardExtension.saveDashboard().then(window.history.back());
        }
        this.onSavePopupNoClick = function () {
            _this.savePopupVisible(false);
            window.history.back();
        }
        this.onSavePopupCancelClick = function () {
            _this.savePopupVisible(false);
        }        
    }
    ExitDashboardExtension.prototype.start = function () {
        this.toolboxExtension.menuItems.push(this._exitMenuItem);
        this.toolboxExtension.menuItems().filter(function (item) { return item.id === 'save-as' })[0].hasSeparator = false;
    };
    ExitDashboardExtension.prototype.stop = function () {
        this.toolboxExtension.menuItems.remove(this._exitMenuItem);
        this.toolboxExtension.menuItems().filter(function (item) { return item.id === 'save-as' })[0].hasSeparator = true;
    };

    CustomExtensions.ExitDashboardExtension = ExitDashboardExtension;
})(CustomExtensions || (CustomExtensions = {}));