document.addEventListener('DOMContentLoaded', function () {
    const translateButton = document.getElementById("translator-translate")
    if (translateButton) {
        translateButton.addEventListener('click', translateClick);
    }

    const transliterateButton = document.getElementById("translator-transliterate")
    if (transliterateButton) {
        transliterateButton.addEventListener('click', transliterateClick);
    }
});

function getTranslateData() {
    const input = document.getElementById("translator-input");
    if (!input) throw "#transator-input bot found";

    const text = input.value.trim();
    if (input.value.length == 0) {
        alert("Введите текст для перевода");
        input.focus();
        return;
    }
    const output = document.getElementById("translator-output");
    if (!output) throw "#transator-output bot found";

    const lang_from = document.getElementById("translator-lang-from");
    if (!lang_from) throw "#transator-lang-from bot found";

    const lang_to = document.getElementById("translator-lang-to");
    if (!lang_to) throw "#transator-lang-to bot found";

    return [text, lang_from, lang_to, output];
}
function translateClick() {

    let [text, lang_from, lang_to, output] = getTranslateData();
    //output.value = input.value + ' ' + lang_from.value + ' ' + lang_to.value;

    fetch(`/api/translate?text=${text}&from=${lang_from.value}&to=${lang_to.value}`).then(r => r.json()).then(j => {
        console.log(j);
        output.value = j[0].translations[0].text;
    });
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
    }).then(r => r.text()).then(j => {
        console.log(j);
        //output.value = j[0].translations[0].text;
    });
}