(function (global) {
    function AssigneeProvider(propertiesPanel, translate) {
        return {
            getTabs: function (element) {
                if (element.type !== 'bpmn:UserTask') return [];

                return [
                    {
                        id: 'assignee-tab',
                        label: 'Kullanıcı',
                        groups: [
                            {
                                id: 'assignee-group',
                                label: 'Atama',
                                entries: [
                                    {
                                        id: 'assignee',
                                        html: '<input id="assigneeInput" type="text" />'
                                    }
                                ]
                            }
                        ]
                    }
                ];
            }
        };
    }

    AssigneeProvider.$inject = ['propertiesPanel', 'translate'];

    global.CustomAssigneeProvider = {
        __init__: ['assigneeProvider'],
        assigneeProvider: ['type', AssigneeProvider]
    };
})(window);
