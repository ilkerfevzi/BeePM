export default function CustomAssigneeProvider(propertiesPanel, modeling, elementRegistry) {
    propertiesPanel.registerProvider(500, {
        getGroups(element) {
            if (element.type === 'bpmn:UserTask') {
                return function (groups) {
                    // "General" grubuna yeni alan ekle
                    const generalGroup = groups.find(g => g.id === 'general');

                    if (generalGroup) {
                        generalGroup.entries.push({
                            id: 'assignee',
                            label: 'Assignee (Dropdown)',
                            html: `
                <select id="assigneeSelect" style="width:100%"></select>
              `,
                            get(element, node) {
                                return { assignee: element.businessObject.$attrs['camunda:assignee'] || '' };
                            },
                            set(element, values, node) {
                                modeling.updateProperties(element, {
                                    'camunda:assignee': values.assignee
                                });
                                return { assignee: values.assignee };
                            },
                            mounted(element, node) {
                                // Select2 bağla
                                $('#assigneeSelect', node).select2({
                                    ajax: {
                                        url: '/FlowDesigner/Users',
                                        dataType: 'json',
                                        processResults: function (data) {
                                            return { results: data };
                                        }
                                    }
                                });

                                // mevcut değer varsa set et
                                const currentVal = element.businessObject.$attrs['camunda:assignee'];
                                if (currentVal) {
                                    const option = new Option(currentVal, currentVal, true, true);
                                    $('#assigneeSelect', node).append(option).trigger('change');
                                }

                                // change event → BPMN attribute güncelle
                                $('#assigneeSelect', node).on('change', function () {
                                    const val = $(this).val();
                                    modeling.updateProperties(element, { 'camunda:assignee': val });
                                });
                            }
                        });
                    }
                    return groups;
                };
            }
        }
    });
}

CustomAssigneeProvider.$inject = ['propertiesPanel', 'modeling', 'elementRegistry'];
