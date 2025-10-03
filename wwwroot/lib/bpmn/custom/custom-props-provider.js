import { TextFieldEntry, isTextFieldEntryEdited } from 'bpmn-js-properties-panel';

export default function CustomPropsProvider(propertiesPanel, translate) {
    return {
        getGroups(element) {
            return function (groups) {

                // Sadece UserTask için ekle
                if (element.type === 'bpmn:UserTask') {
                    groups.push({
                        id: 'customProps',
                        label: 'Custom Properties',
                        entries: [

                            // Assignee alanı
                            {
                                id: 'assignee',
                                element,
                                component: TextFieldEntry,
                                getValue: () => element.businessObject.assignee || '',
                                setValue: (val) => element.businessObject.assignee = val,
                                isEdited: isTextFieldEntryEdited
                            },

                            // FormKey alanı
                            {
                                id: 'formKey',
                                element,
                                component: TextFieldEntry,
                                getValue: () => element.businessObject.formKey || '',
                                setValue: (val) => element.businessObject.formKey = val,
                                isEdited: isTextFieldEntryEdited
                            }

                        ]
                    });
                }

                return groups;
            };
        }
    };
}
