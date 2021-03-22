var CustomExtensions;
(function (CustomExtensions) {
    function SaveAsDashboardExtension(dashboardControl) {
        var _this = this;

        this.name = 'save-as';
        this.dashboardControl = dashboardControl;
        this.toolboxExtension = this.dashboardControl.findExtension('toolbox');
        this.createDashboardExtension = this.dashboardControl.findExtension('create-dashboard');
        this.template = 'dashboard-save-as-extension'

        this._saveAsMenuItem = new DevExpress.Dashboard.DashboardMenuItem('save-as', 'Save As...', 120, 0, function () {
            var newName = _this.dashboardControl.dashboard().title.text() + ' - Copy';

            _this.savePopupNewName(newName);
            _this.savePopupVisible(true);
        });
        this._saveAsMenuItem.hasSeparator = true;
        this._saveAsMenuItem.data = _this;
        this.savePopupVisible = ko.observable(false);
        this.savePopupNewName = ko.observable('');

        this.onSavePopupOkClick = function () {
            _this.savePopupVisible(false);
            _this.createDashboardExtension.performCreateDashboard(_this.savePopupNewName(), _this.dashboardControl.dashboard().getJSON());
        };
        this.onSavePopupCancelClick = function () {
            _this.savePopupVisible(false);
        }        
    }
    SaveAsDashboardExtension.prototype.start = function () {
        this.toolboxExtension.menuItems.push(this._saveAsMenuItem);
        this.toolboxExtension.menuItems().filter(function (item) { return item.id === 'save' })[0].hasSeparator = false;
    };
    SaveAsDashboardExtension.prototype.stop = function () {
        this.toolboxExtension.menuItems.remove(this._saveAsMenuItem);
        this.toolboxExtension.menuItems().filter(function (item) { return item.id === 'save' })[0].hasSeparator = true;
    };

    CustomExtensions.SaveAsDashboardExtension = SaveAsDashboardExtension;
})(CustomExtensions || (CustomExtensions = {}));