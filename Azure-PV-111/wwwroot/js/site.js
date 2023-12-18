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

    const linkButton = document.getElementById("db-button-link")
    if (linkButton) {
        linkButton.addEventListener('click', linkButtonClick);
    }

    const addProductButton = document.getElementById("db-add-product")
    if (addProductButton) {
        addProductButton.addEventListener('click', addProductClick);
    }

    loadProducers();
});

function addProductClick() {
    const productContainer = document.getElementById('db-product-container')
    if (!productContainer) throw "#db-product-container not found";
    const producerId = productContainer.getAttribute('data-producer-id');
    if (!producerId) {
        alert("Select producer");
        return;
    }
    console.log(producerId);
    const nameInput = document.querySelector('[name="db-product-name"]');
    if (!nameInput) throw '[name="db-product-name"] not found';
    const yearInput = document.querySelector('[name="db-product-year"]');
    if (!yearInput) throw '[name="db-product-year"] not found';
    fetch('/api/db', {
        method: 'ADD',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            producerId: producerId,
            name: nameInput.value,
            year: yearInput.value
        })
    }).then(r => r.text()).then(console.log);
}
function linkButtonClick() {
    fetch('/api/db', { method: 'LINK' }).then(r => r.text()).then(console.log);
}
function findProducerData(e) {
    const idCarrier = e.target.closest('[producer-id]');
    if (!idCarrier) throw "producer-id not found";
    const producerId = idCarrier.getAttribute('producer-id');
    const nameCarrier = idCarrier.querySelector('[data-name]');
    return { producerId, nameCarrier, idCarrier };
}
function editProducerClick(e) {
    const { producerId, nameCarrier, idCarrier } = findProducerData(e);
    const newName = prompt("Enter new title", nameCarrier.innerText);
    if (newName != nameCarrier.innerText && newName !== "" && newName !== null) {
        fetch(`/api/db?producerId=${producerId}&newName=${newName}`, {
            method: "PUT"
        })
            .then(r => r.json())
            .then(j => {
                console.log(j);
                if (j.status == 200) {
                    nameCarrier.innerText = newName;
                }
                else {
                    alert("Error while changing");
                }
            });
    }
    else {
        alert("Changes denied");
    }
    console.log("edit " + producerId + " " + nameCarrier.innerText);
}
function loadProducers() {
    const container = document.getElementById("db-producers-container");
    if (!container) return;
    fetch("/api/db?type=Producer")
        .then(r => r.json())
        .then(j => {
            const table = document.createElement('table');
            const tbody = document.createElement('tbody');
            for (let item of j) {
                let tr, td, tn, btn, i, radio;
                tr = document.createElement('tr');
                tr.setAttribute('producer-id', item.id);

                td = document.createElement('td');
                td.setAttribute('name', '');
                tn = document.createTextNode(item.name);
                td.appendChild(tn);
                tr.appendChild(td);

                td = document.createElement('td');
                radio = document.createElement('input');
                radio.setAttribute('type', 'radio');
                radio.setAttribute('name', 'radio-producer');
                radio.setAttribute('value', item.id);
                radio.addEventListener('change', radioProducerChanged);
                td.appendChild(radio);
                tr.appendChild(td);

                td = document.createElement('td');
                btn = document.createElement('button');
                btn.classList.add('btn', 'btn-danger');
                i = document.createElement('i');
                i.classList.add('bi', 'bi-trash3-fill');
                btn.appendChild(i);
                btn.addEventListener('click', deleteProducerClick);
                td.appendChild(btn);
                tr.appendChild(td);
                tbody.appendChild(tr);
                // edit button
                td = document.createElement('td');
                btn = document.createElement('button');
                btn.classList.add('btn', 'btn-warning');
                // <i class="bi bi-pen-fill"></i>
                i = document.createElement('i');
                i.classList.add('bi', 'bi-pen-fill');
                btn.appendChild(i);
                btn.addEventListener('click', editProducerClick);
                td.appendChild(btn);
                tr.appendChild(td);
            }
            table.appendChild(tbody);
            table.className = "table";
            container.innerHTML = "";
            container.appendChild(table);

            console.log(j);
        });
    //container.innerText = j);
}
function radioProducerChanged(e) {
    loadProducts(e.target.value);
    showProducts();
}
function loadProducts(producerId) {
    const productsContainer = document.getElementById("db-product-container");
    if (!productsContainer) throw ("#db-products-container not found");
    productsContainer.setAttribute('data-producer-id', producerId);
}
function showProducts() {
    const container = document.getElementById("db-product-container");
    if (!container) return;
    fetch("/api/db?type=Producer")
        .then(r => r.json())
        .then(j => {
            const table = document.createElement('table');
            const tbody = document.createElement('tbody');

            j.forEach(producer => {
                if (producer.products && Array.isArray(producer.products)) {
                    producer.products.forEach(product => {
                        let tr, td, tn;
                        tr = document.createElement('tr');
                        tr.setAttribute('product-id', product.id);

                        // Product Name
                        td = document.createElement('td');
                        tn = document.createTextNode(product.name);
                        td.appendChild(tn);
                        tr.appendChild(td);

                        // Year
                        td = document.createElement('td');
                        tn = document.createTextNode(product.year);
                        td.appendChild(tn);
                        tr.appendChild(td);

                        tbody.appendChild(tr);
                    });
                }
            });

            table.appendChild(tbody);
            table.className = "table";
            container.innerHTML = "";
            container.appendChild(table);

            console.log(j);
        })
        .catch(error => console.error('Error fetching data:', error));
}
function deleteProducerClick(e) {
    const idCarrier = e.target.closest('[producer-id]');
    if (!idCarrier) throw "producer-id not found";
    const producerId = idCarrier.getAttribute('producer-id');
    const nameCarrier = idCarrier.querySelector('[data-name]');
    if (confirm('Do you really want to delete ' + nameCarrier.innerText)) {
        console.log("to delete " + producerId);
        fetch(`/api/db?producerId=${producerId}`, {
            method: "DELETE"
        }).then(r => r.json())
            .then(j => {
                if (j.status == 204) {
                    alert("Delete OK!");
                    location.reload();

                }
                else {
                    alert("Something gone wrong!");
                }
            });
    }
    //location.reload();

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
            body: JSON.stringify({ name: nameInput.value })
        }).then(r => r.json()).then(j => {
            console.log(j),
                location.reload()
        });
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
    delayedAction = setTimeout(translateSelection, 1000);
    //console.log(e);
    //console.log(document.getSelection().toString());
}

function translateSelection() {
    const check = document.getElementById("translator-selection");
    const check2 = document.getElementById("transliterator-selection");
    let text = document.getSelection().toString().trim();
    let [_, lang_from, lang_to, output] = getTranslateData();

    let x;
    let y;
    const fromScript = lang_from.selectedOptions[0].getAttribute("data-script");
    const toScript = "Latn";
    if (text.length > 0 && check.checked && check2.checked) {
        fetch(`/api/translate?text=${text}&from=${lang_from.value}&to=${lang_to.value}`, {
            method: "GET"
        })
            .then(r => r.json())
            .then(j => {
                x = j[0].translations[0].text;
                console.log(x);

                // Второй запрос внутри первого
                return fetch(`/api/translate?text=${text}&from=${lang_from.value}&fromScript=${fromScript}&toScript=${toScript}`, {
                    method: "POST"
                });
            })
            .then(r => r.json())
            .then(j => {
                y = j[0].text;
                console.log(y);
                alert(`${x}\n${y}`);
                return;
            });
    }

    else if (text.length > 0 && check.checked) {
        fetch(`/api/translate?text=${text}&from=${lang_from.value}&to=${lang_to.value}`)
            .then(r => r.json())
            .then(j => {
                console.log(j);
                output.value = j[0].translations[0].text;

            });
    }
    else if (check2.checked) {

        if (!fromScript) {
            alert("Транслітерація цієї мови не підтримується");
            return;
        }

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