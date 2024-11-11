const toggleMobileMenu = () => document.querySelector('#mobile-menu').classList.toggle('translate-x-full');

document.querySelector('#toggle-button').addEventListener('click', toggleMobileMenu);
document.querySelector('#close-mobile-menu-button').addEventListener('click', toggleMobileMenu);

const searchInput = document.getElementById("searchInput");
const searchSuggestions = document.getElementById("searchSuggestions");

var data = 0;
async function SimpleSearch(input) {
    let inputTrimmed = input.trim().toLowerCase();
    if (inputTrimmed.length <= 2) return;
    if (data === 0) {
        data = await fetch('Index.json')
            .then(response => {
                return response.json();
            });
    }
    var results = [];
    for (let key in data.content)
        if (data.content.hasOwnProperty(key) && key.includes(inputTrimmed)) {
            // assuming that the title is the first entry
            // it can probably be moved to the url section
            results.push(
                {
                    name: key.slice(0, key.indexOf("*-*")),
                    url: data.url[data.content[key]]
                }
            );
        }
    console.log("include: " + results.length);
    return results;
}

searchInput.addEventListener('input', async function () {
    console.log(searchInput.value);
    let results = await SimpleSearch(searchInput.value);
    if (results === undefined) { return; }
    searchSuggestions.innerHTML = "<ul>";
    for (let i = 0; i < results.length; i++) {
        searchSuggestions.innerHTML += `<li><a href="${results[i].url}">${results[i].name}</a></li>`;
    }
    searchSuggestions.innerHTML += "</ul>";
});