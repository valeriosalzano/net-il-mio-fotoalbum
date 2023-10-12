const id = "SelectedCategoriesId";

const selectFilter = document.querySelector(`#${id}Filter`);

const selectOptions = document.querySelectorAll(`#${id} > option`);

if (selectFilter) {
    selectFilter.addEventListener("keyup", () => {
        let filterValue = selectFilter.value;
        selectOptions.forEach(option => {
            console.log(option);
            if (!option.innerHTML.match(new RegExp(`${filterValue}`, 'i'))) {
                option.classList.add("d-none");
            } else {
                option.classList.remove("d-none");
            }
        })
    })
}