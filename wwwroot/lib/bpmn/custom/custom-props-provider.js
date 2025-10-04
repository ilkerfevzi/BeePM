(function (global) {
    function CustomPropsProvider(propertiesPanel, translate) {
        return {
            getTabs: function (element) {
                return [
                    {
                        id: 'custom-tab',
                        label: 'Custom',
                        groups: [
                            {
                                id: 'custom-group',
                                label: 'Ek Alanlar',
                                entries: [
                                    {
                                        id: 'custom-note',
                                        description: 'Özel açıklama',
                                        html: '<input id="customNote" type="text" />'
                                    }
                                ]
                            }
                        ]
                    }
                ];
            }
        };
    }

    CustomPropsProvider.$inject = ['propertiesPanel', 'translate'];

    global.CustomPropsProvider = {
        __init__: ['customPropsProvider'],
        customPropsProvider: ['type', CustomPropsProvider]
    };
})(window);
