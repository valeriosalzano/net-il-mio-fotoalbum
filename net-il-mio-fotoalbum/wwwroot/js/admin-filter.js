const filterInputDOM = document.getElementById("filter-input");
const filterWrappersDOM = document.querySelectorAll(".filter-wrapper");

if (filterInputDOM) {
    filterInputDOM.addEventListener("keyup", () => {
        let filterValue = filterInputDOM.value.toLowerCase();
        if (filterWrappersDOM && filterValue != "") {
            filterWrappersDOM.forEach(element => {
                let filterTitle = element.querySelector(".filter-title").innerHTML.toLowerCase();
                if (filterTitle.includes(filterValue)) {
                    element.classList.remove("d-none");
                } else {
                    element.classList.add("d-none");
                }
            })
        }
    })
}
