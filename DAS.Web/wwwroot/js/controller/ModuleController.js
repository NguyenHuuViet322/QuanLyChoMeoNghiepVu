const icons = [
    'fas fa-address-book',
    'far fa-address-book',
    'fas fa-address-card',
    'far fa-address-card',
    'fas fa-adjust',
    'fas fa-align-center',
    'fas fa-align-justify',
    'fas fa-align-left',
    'fas fa-align-right',
    'fas fa-angle-double-down',
    'fas fa-angle-double-left',
    'fas fa-angle-double-right',
    'fas fa-angle-double-up',
    'fas fa-archive',
    'fas fa-arrow-alt-circle-down',
    'far fa-arrow-alt-circle-down',
    'fas fa-arrow-alt-circle-left',
    'far fa-arrow-alt-circle-left',
    'fas fa-arrow-alt-circle-right',
    'far fa-arrow-alt-circle-right',
    'fas fa-arrow-alt-circle-up',
    'far fa-arrow-alt-circle-up',
    'fas fa-arrow-circle-down',
    'fas fa-arrow-circle-left',
    'fas fa-arrow-circle-right',
    'fas fa-arrow-circle-up',
    'fas fa-arrow-down',
    'fas fa-arrow-left',
    'fas fa-arrow-right',
    'fas fa-arrow-up',
    'fas fa-arrows-alt',
    'fas fa-arrows-alt-h',
    'fas fa-arrows-alt-v',
    'fas fa-asterisk',
    'fas fa-at',
    'fas fa-atlas',
    'fas fa-atom',
    'fas fa-award',
    'fas fa-backspace',
    'fas fa-backward',
    'fas fa-forward',
    'fas fa-balance-scale',
    'fas fa-ban',
    'fas fa-barcode',
    'fas fa-bars',
    'fas fa-bell',
    'far fa-bell',
    'fas fa-binoculars',
    'fas fa-book',
    'fas fa-book-medical',
    'fas fa-book-open',
    'fas fa-book-reader',
    'fas fa-bookmark',
    'far fa-bookmark',
    'fas fa-border-all',
    'fas fa-box',
    'fas fa-box-open',
    'fas fa-briefcase',
    'fas fa-briefcase-medical',
    'fas fa-building',
    'far fa-building',
    'fas fa-bullhorn',
    'fas fa-bullseye',
    'fas fa-bus',
    'fas fa-bus-alt',
    'fas fa-business-time',
    'fas fa-calculator',
    'fas fa-calendar',
    'far fa-calendar',
    'fas fa-calendar-alt',
    'far fa-calendar-alt',
    'fas fa-calendar-check',
    'far fa-calendar-check',
    'fas fa-calendar-day',
    'fas fa-calendar-minus',
    'far fa-calendar-minus',
    'fas fa-calendar-plus',
    'far fa-calendar-plus',
    'fas fa-calendar-times',
    'far fa-calendar-times',
    'fas fa-calendar-week',
    'fas fa-camera',
    'fas fa-camera-retro',
    'fas fa-chart-area',
    'fas fa-chart-bar',
    'far fa-chart-bar',
    'fas fa-chart-line',
    'fas fa-chart-pie',
    'fas fa-check',
    'fas fa-check-circle',
    'far fa-check-circle',
    'fas fa-check-square',
    'far fa-check-square',
    'fas fa-clipboard',
    'far fa-clipboard',
    'fas fa-clipboard-check',
    'fas fa-clipboard-list',
    'fas fa-clock',
    'far fa-clock',
    'fas fa-clone',
    'far fa-clone',
    'fas fa-cloud',
    'fas fa-cloud-download-alt',
    'fas fa-cloud-upload-alt',
    'fas fa-code',
    'fas fa-code-branch',
    'fas fa-cog',
    'fas fa-cogs',
    'fas fa-coins',
    'fas fa-columns',
    'fas fa-comment',
    'far fa-comment',
    'fas fa-comment-alt',
    'far fa-comment-alt',
    'fas fa-comment-dots',
    'far fa-comment-dots',
    'fas fa-comment-slash',
    'fas fa-comments',
    'far fa-comments',
    'fas fa-copy',
    'far fa-copy',
    'fas fa-copyright',
    'far fa-copyright',
    'fas fa-database',
    'fas fa-desktop',
    'fas fa-download',
    'fas fa-edit',
    'far fa-edit',
    'fas fa-eject',
    'fas fa-ellipsis-h',
    'fas fa-ellipsis-v',
    'fas fa-envelope',
    'far fa-envelope',
    'fas fa-envelope-open',
    'far fa-envelope-open',
    'fas fa-envelope-open-text',
    'fas fa-eraser',
    'fas fa-exchange-alt',
    'fas fa-exclamation',
    'fas fa-exclamation-circle',
    'fas fa-exclamation-triangle',
    'fas fa-external-link-alt',
    'fas fa-eye',
    'far fa-eye',
    'fas fa-eye-dropper',
    'fas fa-eye-slash',
    'far fa-eye-slash',
    'fas fa-fast-backward',
    'fas fa-fast-forward',
    'fas fa-fax',
    'fas fa-file-download',
    'fas fa-file-export',
    'fas fa-file-import',
    'fas fa-file-upload',
    'fas fa-file',
    'far fa-file',
    'fas fa-file-alt',
    'far fa-file-alt',
    'fas fa-file-archive',
    'far fa-file-archive',
    'fas fa-file-code',
    'far fa-file-code',
    'fas fa-file-csv',
    'fas fa-file-excel',
    'far fa-file-excel',
    'fas fa-file-powerpoint',
    'far fa-file-powerpoint',
    'fas fa-file-word',
    'far fa-file-word',
    'fas fa-file-pdf',
    'far fa-file-pdf',
    'fas fa-file-audio',
    'far fa-file-audio',
    'fas fa-file-image',
    'far fa-file-image',
    'fas fa-file-video',
    'far fa-file-video',
    'fas fa-filter',
    'fas fa-fingerprint',
    'fas fa-flag',
    'far fa-flag',
    'fas fa-folder',
    'far fa-folder',
    'fas fa-folder-open',
    'far fa-folder-open',
    'fas fa-hdd',
    'far fa-hdd',
    'fas fa-image',
    'far fa-image',
    'fas fa-images',
    'far fa-images',
    'fas fa-inbox',
    'fas fa-info',
    'fas fa-info-circle',
    'fas fa-key',
    'fas fa-laptop',
    'fas fa-layer-group',
    'fas fa-list',
    'fas fa-list-ol',
    'fas fa-list-ul',
    'fas fa-lock',
    'fas fa-lock-open',
    'fas fa-unlock',
    'fas fa-network-wired',
    'fas fa-paper-plane',
    'fas fa-paperclip',
    'fas fa-paste',
    'fas fa-pen',
    'fas fa-pen-alt',
    'fas fa-pen-square',
    'fas fa-pencil-alt',
    'fas fa-pencil-ruler',
    'fas fa-people-arrows',
    'fas fa-power-off',
    'fas fa-print',
    'fas fa-project-diagram',
    'fas fa-qrcode',
    'fas fa-question',
    'fas fa-question-circle',
    'far fa-question-circle',
    'fas fa-save',
    'far fa-save',
    'fas fa-search',
    'fas fa-sd-card',
    'fas fa-server',
    'fas fa-signal',
    'fas fa-signature',
    'fas fa-sitemap',
    'fas fa-sliders-h',
    'fas fa-spell-check',
    'fas fa-stamp',
    'fas fa-suitcase',
    'fas fa-tasks',
    'fas fa-th-list',
    'fas fa-tools',
    'fas fa-trash',
    'fas fa-trash-alt',
    'far fa-trash-alt',
    'fas fa-user-check',
    'fas fa-user-cog',
    'fas fa-user-edit',
    'fas fa-user-lock',
];
const IconPicker = {
    $_container: jQuery('<div />').attr({ 'data-toggle': 'buttons', class: 'btn-group btn-group-toggle flex-wrap justify-content-center' }),
    $_label: jQuery('<label />').attr({ class: 'btn btn-outline-dark btn-hover-success m-1 flex-grow-0' }),
    $_input: jQuery('<input />').attr({ type: 'radio', name: 'icon', autocomplete: 'off' }),
    $_icon: jQuery('<i />').attr({ class: 'fa-2x fa-fw' }),
    modal: '#mdIconPicker',
    btnSelect: '#icon_select',
    targetIcon: '#targetIcon .input-group-prepend i',
    targetInput: '#targetIcon .form-control',
    iconDefault: 'far fa-file-alt',
    noIcons: 'No icon available',

    /**
     * _createIcons
     * @param {string[]} icons
     */
    _createIcons: function (icons) {
        $(this.modal).find('.modal-body').append(this.$_container);
        let items = [];
        for (let icon of icons) {
            let _icon = this.$_icon.clone().addClass(icon);
            let _input = this.$_input.clone().attr({ value: icon });
            let _label = this.$_label.clone().append(_input, _icon);
            items.push(_label);
        }
        this.$_container.append(items);
        return items;
    },
    _getSelectedIcon: function () {
        return this.$_container.find('.active').children('input').val() || this.iconDefault;
    },
    _setSelectedIcon: function () {
        const selected = this._getSelectedIcon();
        $(this.targetIcon).attr({ class: selected });
        $(this.targetInput).val(selected);
        return selected;
    },
    /**
     * init
     * @param {string[]} icons 
     */
    init: function (icons) {
        if (icons.length > 0) {
            this._createIcons(icons);
            var self = this;
            $(this.btnSelect).on('click', function () {
                self._setSelectedIcon();
                $(self.modal).modal('hide');
            });
        } else {
            $(this.modal).find('.modal-body').append(this.noIcons);
            return false;
        }
    }
}
IconPicker.init(icons);
//const ModuleConfig = {
//    Select2Init: function () {
//        function formatResult(node) {
//            if (typeof (node.element) == undefined) {
//                return node.text;
//            }
//            var level = 0;
//            if (node.element !== undefined) {
//                var levelvalue = (node.element.getAttribute("level"));
//                if (levelvalue == null) level = 0;
//                else level = levelvalue;
//            }
//            var $result = $('<span style="padding-left:' + (20 * level) + 'px;">' + node.text + '</span>');
//            return $result;
//        };
//        $('.select2tree').select2({
//            placeholder: function () {
//                $(this).data('placeholder');
//            },
//            width: function () {
//                $(this).data('width');
//            },
//            language: "vi",
//            templateResult: formatResult,
            
//        });      
//    },
//};