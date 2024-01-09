/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // Define changes to default configuration here. For example:
    config.language = 'zh';
    //config.uiColor = '#AADC6E';
    config.toolbar = 'BasicToolbar';
    config.startupMode = 'source';
    //config.startupMode = 'wysiwyg';
    config.enterMode = CKEDITOR.ENTER_BR;
    config.shiftEnterMode = CKEDITOR.ENTER_P;
    config.allowedContent = true;
    CKEDITOR.config.fillEmptyBlocks = false;
    CKEDITOR.config.fullPage = false;
    CKEDITOR.dtd.$removeEmpty['i'] = false;
    //CKEDITOR.on('instanceReady', function (ev) {
    //    var blockTags = ['div', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'p', 'pre', 'li', 'blockquote', 'ul', 'ol',
    //        'table', 'thead', 'tbody', 'tfoot', 'td', 'th',];

    //    for (var i = 0; i < blockTags.length; i++) {
    //        ev.editor.dataProcessor.writer.setRules(blockTags[i], {
    //            indent: false,
    //            breakBeforeOpen: false,
    //            breakAfterOpen: false,
    //            breakBeforeClose: false,
    //            breakAfterClose: false
    //        });
    //    }
    //});
    CKEDITOR.on('instanceReady', function (ev) {
        ev.editor.dataProcessor.writer.setRules('p',
            {
                indent: false,
                breakBeforeOpen: true,
                breakAfterOpen: false,
                breakBeforeClose: false,
                breakAfterClose: true
            });
    });
    config.toolbar_BasicToolbar = [['Source', '-', 'NewPage', 'Preview', '-', 'Templates'],
    ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Print', 'SpellChecker', 'Scayt'],
    ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
    ['Link', 'Unlink', 'Anchor'],
    ['Undo', 'Redo', '-', 'Find', 'Replace', '-', 'SelectAll', 'RemoveFormat'],
    ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
        '/',
    ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
        ['Styles', 'Format', 'Font', 'FontSize'],

    ['TextColor', 'BGColor'],
    ['Image', 'Table', 'HorizontalRule', 'SpecialChar', 'PageBreak'],
    ['Maximize', 'ShowBlocks']];

    //config.toolbar_SimpleToolbar = [
    //    ['Source', '-', 'NewPage', 'Preview'],
    //    ['Cut', 'Copy', 'Paste'],
    //    ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
    //    ['Link', 'Unlink', 'Anchor'],
    //    ['Undo', 'Redo', '-', 'Find', 'Replace'],
    //    ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
    //    ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
    //    ['TextColor', 'BGColor'],
    //    ['Image', 'Table', 'HorizontalRule'],
    //    ['Maximize']
    //];
};
