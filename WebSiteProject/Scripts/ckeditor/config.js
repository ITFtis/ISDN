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

    config.allowedContent = true;
    CKEDITOR.config.fillEmptyBlocks = false;
    CKEDITOR.config.fullPage = false;
    CKEDITOR.dtd.$removeEmpty['i'] = false;

    config.toolbar_BasicToolbar = [['Source', '-', 'NewPage', 'Preview'],
    ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Print', 'SpellChecker', 'Scayt'],
    ['Link', 'Unlink', 'Anchor'],
    ['Undo', 'Redo', '-', 'Find', 'Replace', '-', 'SelectAll', 'RemoveFormat'],
    ['NumberedList', 'BulletedList', '-', 'Blockquote'],
        '/',
    ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
    ['TextColor', 'BGColor'],
    ['Image', 'Table', 'HorizontalRule', 'SpecialChar'],
    ['Maximize']];

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
