$(document).ready(function () {
    
    $.extend($.tablesorter.themes.jui, {
        // change default jQuery uitheme icons - find the full list of icons here: http://jqueryui.com/themeroller/ (hover over them for their name)
        table: 'ui-widget ui-widget-content ui-corner-all', // table classes
        caption: 'ui-widget-content ui-corner-all',
        header: 'ui-widget-header ui-corner-all ui-state-default', // header classes
        footerRow: '',
        footerCells: '',
        icons: 'ui-icon', // icon class added to the <i> in the header
        sortNone: 'ui-icon-carat-2-n-s',
        sortAsc: 'ui-icon-carat-1-n',
        sortDesc: 'ui-icon-carat-1-s',
        active: 'ui-state-active', // applied when column is sorted
        hover: 'ui-state-hover',  // hover class
        filterRow: '',
        even: 'ui-widget-content', // odd row zebra striping
        odd: 'ui-state-default'   // even row zebra striping
    });

    $('.tablesorter').tablesorter({
       
        theme: 'jui',
        headerTemplate : '{content} {icon}',
        widgets: ['zebra', 'filter', 'uitheme']
    }).tablesorterPager({
        container: $('.pager'),
        output: '{startRow} - {endRow} / {filteredRows} ({totalRows})',
    });
});