const sweetDeleteModal = document.querySelector("#modal-sweet-delete")
const sweetDeleteBtns = document.querySelectorAll(".btn-sweet-delete");

sweetDeleteBtns.forEach(btn => {
    btn.addEventListener('click', event => {
        event.preventDefault();

        const title = btn.closest(".sweet-delete-target-wrapper").querySelector(".sweet-delete-target-title").innerHTML;

        sweetDeleteModal.querySelector("#modal-title").innerHTML = title;
        sweetDeleteModal.classList.remove('d-none');

        const dismissBtn = sweetDeleteModal.querySelector(".btn-dismiss");
        const confirmBtn = sweetDeleteModal.querySelector(".btn-confirm");

        dismissBtn.addEventListener('click', () => {
            sweetDeleteModal.classList.add('d-none');
        })
        confirmBtn.addEventListener('click', () => {
            btn.parentElement.submit();
        })
    })
})