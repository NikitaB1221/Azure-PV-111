document.addEventListener('DOMContentLoaded', function () {
    const translateButton = document.getElementById("translator-translate")
    if (translateButton) {
        translateButton.addEventListener('click', translateClick);
    }

    const switchButton = document.getElementById("translator-switch")
    if (switchButton) {
        switchButton.addEventListener('click', switchClick);
    }

    const transliterateButton = document.getElementById("translator-transliterate")
    if (transliterateButton) {
        transliterateButton.addEventListener('click', transliterateClick);
    }

    if (translateButton) {
        document.addEventListener("selectionchange", onSelectionChange);
    }

    const addProducerButton = document.getElementById("db-add-producer")
    if (addProducerButton) {
        addProducerButton.addEventListener('click', addProducerClick);
    }

    loadProducers();
});

function loadProducers() {
    const container = document.getElementById("db-producers-container");
    if (!container) return;
    fetch("/api/db?type=Producer").then(r => r.json()).then(j =>
    {
        console.log(j)
    });
}

function addProducerClick() {
    const nameInput = document.querySelector("input[name='db-producer']");
    if (!nameInput) throw "input[name='db-producer'] Not found";
    fetch("/api/db",
        {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({name: nameInput.value})
        }).then(r => r.json()).then(console.log)
    console.log(nameInput.value);
}

let lastSelectionTimestamp = 0;
let delayedAction;
function onSelectionChange(e) {
    if (e.timeStamp - lastSelectionTimestamp < 300) {
        if (typeof delayedAction != 'undefined') {
            clearTimeout(delayedAction);
        }
    }
    lastSelectionTimestamp = e.timeStamp;
    delayedAction = setTimeout(translateSelection,1000);
    //console.log(e);
    //console.log(document.getSelection().toString());
}

function translateSelection() {
    const check = document.getElementById("translator-selection");
    const check2 = document.getElementById("transliterator-selection");
    let text = document.getSelection().toString().trim();
    let [_, lang_from, lang_to, output] = getTranslateData();
    if (text.length > 0 && check.checked) {
        fetch(`/api/translate?text=${text}&from=${lang_from.value}&to=${lang_to.value}`)
            .then(r => r.json())
            .then(j => {
                console.log(j);
                output.value = j[0].translations[0].text;
            });
    }
    else if (check2.checked) {
        const fromScript = lang_from.selectedOptions[0].getAttribute("data-script");
        if (!fromScript) {
            alert("Транслітерація цієї мови не підтримується");
            return;
        }
        const toScript = "Latn";
        fetch(
            `/api/translate?text=${text}&from=${lang_from.value}&fromScript=${fromScript}&toScript=${toScript}`, {
            method: "POST"
        })
            .then(r => r.json())
            .then(j => {
                console.log(j);
                alert(`${text} ==> ${j[0].text}`);
            });
    }
    delayedAction = undefined;

}

function getTranslateData() {
    const input = document.getElementById("translator-input");
    if (!input) throw "#transator-input bot found";

    const text = input.value.trim();
    //if (input.value.length == 0) {
    //    alert("Введите текст для перевода");
    //    input.focus();
    //    return;
    //}
    const output = document.getElementById("translator-output");
    if (!output) throw "#transator-output bot found";

    const lang_from = document.getElementById("translator-lang-from");
    if (!lang_from) throw "#transator-lang-from bot found";

    const lang_to = document.getElementById("translator-lang-to");
    if (!lang_to) throw "#transator-lang-to bot found";

    return [text, lang_from, lang_to, output];
}

function switchClick() {
    let [text, lang_from, lang_to, output] = getTranslateData();

    const input = document.getElementById("translator-input");
    if (!input) throw "#transator-input bot found";

    if (output.value.trim().length == 0 && text.trim().length != 0) {
        alert("Translate first!");
        return;
    }

    console.log(" | 1LF.V " + lang_from.selectedOptions[0].value + " | 1LT.V " + lang_to.selectedOptions[0].value);
    console.log(" | 1I.V " + input.value + " | 1O.V " + output.value);
    console.log(" | 1LF.IT " + lang_from.selectedOptions[0].innerText + " | LT.IT " + lang_to.selectedOptions[0].innerText);

    //let tmp_lang_val = lang_to.selectedOptions[0].value;
    //lang_to.selectedOptions[0].value = lang_from.selectedOptions[0].value;
    //lang_from.selectedOptions[0].value = tmp_lang_val;

    let tmp_lang_val = lang_to.value;
    lang_to.value = lang_from.value;
    lang_from.value = tmp_lang_val;

    //tmp_lang_val = lang_to.selectedOptions[0].innerText;
    //lang_to.selectedOptions[0].innerText = lang_from.selectedOptions[0].innerText;
    //lang_from.selectedOptions[0].innerText = tmp_lang_val;

    let tmp_txt = input.value;
    input.value = output.value;
    output.value = tmp_txt;

    text = input.value;

    console.log(" | 2LF.V " + lang_from.selectedOptions[0].value + " | 2LT.V " + lang_to.selectedOptions[0].value);
    console.log(" | 2I.V " + input.value + " | 2O.V " + output.value);
    console.log(" | 2LF.IT " + lang_from.selectedOptions[0].innerText + " | 2LT.IT " + lang_to.selectedOptions[0].innerText);

    if (text.trim().length != 0) {
        fetch(`/api/translate?text=${text}&from=${lang_from.value}&to=${lang_to.value}`).then(r => r.json()).then(j => {
            console.log(j);
            output.value = j[0].translations[0].text;
        });
    }
}

function translateClick() {
    let [text, langFrom, langTo, output] = getTranslateData();
    fetch(`/api/translate?text=${text}&from=${langFrom.value}&to=${langTo.value}`)
        .then(r => r.json())
        .then(j => {
            const data = j[0];
            console.log(j);
            if (typeof data['detectedLanguage'] !== 'undefined') {
                langFrom.value = data['detectedLanguage']['language'];

                const lang = data['detectedLanguage']['language'];

                let opts = langFrom.querySelectorAll(`option[disabled]`);
                for (let op of opts) {
                    langFrom.removeChild(op);
                }
                let opt = document.createElement('option');
                opt.disabled = true;
                opt.selected = true;
                opt.value = lang;
                opt.innerText = langFrom.querySelector(`option[value=${lang}]`)
                    .innerText + ' (detected)';
                langFrom.options.add(opt);
            }
            output.value = data.translations[0].text;
        });

    // output.value = input.value + ' ' + langFrom.value + ' ' + langTo.value;
    // "translator-switch" "translator-lang-from" "translator-lang-to"
}

function transliterateClick() {

    let [_, __, lang_to, output] = getTranslateData();
    const fromScript = lang_to.selectedOptions[0].getAttribute("data-script");
    const text = output.value;
    if (!text) {
        alert("Translate first!");
        return;
    }
    if (!fromScript) {
        alert("Transliteration of these languages is not available!");
        return;
    }
    const toScript = "Latn";

    fetch(`/api/translate?text=${text}&from=${lang_to.value}&fromScript=${fromScript}&toScript=${toScript}`, {
        method: "POST"
    }).then(r => r.json()).then(j => {
        console.log(j);
        output.value += "\n\n" + j[0].text;
    });
}