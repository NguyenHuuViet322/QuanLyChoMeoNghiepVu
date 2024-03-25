function initComplete(settings, json) {
    var wrapper = $(settings.nTableWrapper);
    var length = wrapper.find('.dataTables_length');
    var lengthSelect = length.find('.custom-select');
    var tableScroll = wrapper.find('.table-scroll');
    if (settings.aLengthMenu) {
        length.addClass('d-flex');
        lengthSelect.select2({
            dropdownParent: length.find('label'),
            minimumResultsForSearch: 10
        });
    }
    tableScroll.overlayScrollbars({
        className: 'os-theme-dark',
        sizeAutoCapable: true,
        scrollbars: {
            clickScrolling: true
        }
    });
    
    var maxHeight = tableScroll.offset().top + length.outerHeight();
    tableScroll.css({
        'max-height': `calc(100vh - ${maxHeight}px - ${tableScroll.css('margin-bottom')} - 1rem)`,
    });
}
function drawCallback(settings, json) {
    var pagination = $(settings.nTableWrapper).find('.dataTables_paginate .pagination');
    if ($('body').hasClass('text-sm')) {
        pagination.addClass('pagination-sm');
    }
}
var _dataTableOptions = {
    responsive: false,
    pagingType: 'full_numbers',
    autoWidth: false,
    filtering: false,
    ordering: false,
    dom: "<'table-scroll'tr><'row'<'col-sm-12 col-md-6 d-flex align-items-center'i><'col-sm-12 col-md-6 d-flex justify-content-end align-items-center'lp>>",
    language: { url: '../libBid/dist/js/configs/datatable/vi_VN.json' },
    initComplete: initComplete,
    drawCallback: drawCallback,
    pageLength: 10,
    lengthMenu: [10, 20, 50, 100],
    processing: true,
    fixedHeader: true
}