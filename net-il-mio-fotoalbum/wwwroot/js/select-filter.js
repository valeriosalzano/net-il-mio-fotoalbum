const selectFilter = document.querySelector("#select-filter-input");

if (selectFilter) {
    const selectOptions = document.querySelectorAll("#SelectedCategoriesId > option");
    selectFilter.addEventListener("keyup", () => {
        let filterValue = selectFilter.value;
        selectOptions.forEach(option => {
            console.log(option);
            if (option.innerHTML.match(new RegExp(`${filterValue}`, 'i'))) {
                option.classList.remove("d-none");
            } else {
                option.classList.add("d-none");
            }
        })
    })
}