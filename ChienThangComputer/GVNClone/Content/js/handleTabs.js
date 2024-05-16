document.addEventListener("DOMContentLoaded", () => {
    const tabsVertical = document.querySelectorAll('.tab-item-sidebar');
    const panesVertical = document.querySelectorAll('.tab-pane-sidebar');

    const tabActiveVertical = document.querySelector('.tab-item-sidebar.active');
    const lineVertical = document.querySelector('.tabs-sidebar .line-sidebar');
    lineVertical.style.top = tabActiveVertical.offsetTop + "px";
    lineVertical.style.height = tabActiveVertical.offsetHeight + "px";
    tabsVertical.forEach((tab, index) => {
        const pane = panesVertical[index];
        tab.onclick = function () {
            document.querySelector('.tab-item-sidebar.active').classList.remove("active");
            document.querySelector('.tab-pane-sidebar.active').classList.remove("active");
            lineVertical.style.top = this.offsetTop + "px";
            lineVertical.style.height = this.offsetHeight + "px";
            this.classList.add("active");
            pane.classList.add("active");
        }
    });


    const tabsHorizontal = document.querySelectorAll('.tab-item-horizontal');
    const panesHorizontal = document.querySelectorAll('.tab-pane-horizontal');

    const tabActiveHorizontal = document.querySelector('.tab-item-horizontal.active');
    const lineHorizontal = document.querySelector('.tabs-horizontal .line-horizontal');
    lineHorizontal.style.left = tabActiveHorizontal.offsetLeft + "px";
    lineHorizontal.style.width = tabActiveHorizontal.offsetWidth + "px";
    tabsHorizontal.forEach((tab, index) => {
        const pane = panesHorizontal[index];
        tab.onclick = function () {
            document.querySelector('.tab-item-horizontal.active').classList.remove("active");
            document.querySelector('.tab-pane-horizontal.active').classList.remove("active");
            lineHorizontal.style.left = this.offsetLeft + "px";
            lineHorizontal.style.width = this.offsetWidth + "px";
            this.classList.add("active");
            pane.classList.add("active");
        }
    });

    var arrTabs = [
        {
            text: "Tất cả",
        },
        {
            text: "Chờ xác nhận",
        },
        {
            text: "Chờ lấy hàng",
        },
        {
            text: "Đang giao",
        },
        {
            text: "Đã giao",
        },
        {
            text: "Đã huỷ",
        }
    ];


    panesHorizontal.forEach((tab, index) => {
        if (tab.children[0].children[0].className.includes("empty-list"))
        {
            tabsHorizontal[index].innerHTML = arrTabs[index].text + " (0)";
        }
        else
        {
            tabsHorizontal[index].innerHTML = arrTabs[index].text + ` (${tab.children[0].children.length})`;
        }
    })
})