import CustomPropsProvider from '/lib/bpmn/custom/custom-props-provider.js';
import CustomAssigneeProvider from '/lib/bpmn/custom/assignee-provider.js';

const {
    BpmnPropertiesPanelModule,
    BpmnPropertiesProviderModule,
    CamundaPlatformPropertiesProviderModule
} = window.BpmnJSPropertiesPanel;

const bpmnModeler = new BpmnJS({
    container: '#canvas',
    keyboard: { bindTo: window },
    propertiesPanel: { parent: '#properties-panel' },
    additionalModules: [
        BpmnPropertiesPanelModule,
        BpmnPropertiesProviderModule,
        CamundaPlatformPropertiesProviderModule,
        CustomPropsProvider,
        CustomAssigneeProvider
    ],
    moddleExtensions: {
        camunda: await (await fetch('/lib/bpmn/camunda.json')).json()
    }
});

// XML yükleme
(async function () {
    const flowJson = window.flowJsonData || null;
    const defaultDiagram = `<bpmn:definitions xmlns:bpmn="http://www.omg.org/spec/BPMN/20100524/MODEL"
    xmlns:camunda="http://camunda.org/schema/1.0/bpmn"
    targetNamespace="http://bpmn.io/schema/bpmn">
    <bpmn:process id="Process_1" isExecutable="false">
      <bpmn:startEvent id="StartEvent_1" name="Başla" />
      <bpmn:userTask id="Task_1" name="İlk Görev" camunda:assignee="" />
      <bpmn:endEvent id="EndEvent_1" name="Bitir" />
      <bpmn:sequenceFlow id="flow1" sourceRef="StartEvent_1" targetRef="Task_1" />
      <bpmn:sequenceFlow id="flow2" sourceRef="Task_1" targetRef="EndEvent_1" />
    </bpmn:process>
  </bpmn:definitions>`;

    try {
        await bpmnModeler.importXML(flowJson && flowJson !== "{}" ? flowJson : defaultDiagram);
        bpmnModeler.get('canvas').zoom('fit-viewport');
    } catch (err) {
        console.error('BPMN yüklenemedi:', err);
    }
})();

// 🧩 Select2 entegrasyonu (Assignee dropdown)
function enhanceAssigneeSelect() {
    const panel = document.getElementById('properties-panel');
    if (!panel) return;

    const entry = panel.querySelector('[data-entry-id="assignee"] input');
    if (!entry || entry.dataset.enhanced) return;

    entry.dataset.enhanced = '1';
    $(entry).select2({
        placeholder: "Kullanıcı seçin...",
        ajax: {
            url: '/FlowDesigner/Users',
            dataType: 'json',
            delay: 250,
            processResults: data => ({ results: data })
        }
    });
}

const observer = new MutationObserver(() => setTimeout(enhanceAssigneeSelect, 50));
observer.observe(document.getElementById('properties-panel'), { childList: true, subtree: true });
