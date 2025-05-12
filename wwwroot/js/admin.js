document.addEventListener("DOMContentLoaded", () => {
    // Sidebar toggle for mobile
    const sidebarToggle = document.getElementById("sidebarToggle")
    const sidebar = document.getElementById("sidebar")

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener("click", () => {
            sidebar.classList.toggle("-translate-x-full")
        })
    }

    // Initialize charts if ECharts is available
    if (typeof echarts !== "undefined") {
        // Find all chart containers
        const chartContainers = document.querySelectorAll('[id$="Chart"]')

        // Initialize each chart
        chartContainers.forEach((container) => {
            if (container) {
                initializeChart(container.id)
            }
        })
    }

    // Initialize any dropdowns
    const dropdownButtons = document.querySelectorAll("[data-dropdown-toggle]")
    dropdownButtons.forEach((button) => {
        const targetId = button.getAttribute("data-dropdown-toggle")
        const target = document.getElementById(targetId)

        if (target) {
            button.addEventListener("click", (e) => {
                e.stopPropagation()
                target.classList.toggle("hidden")
            })
        }
    })

    // Close dropdowns when clicking outside
    document.addEventListener("click", () => {
        const dropdowns = document.querySelectorAll(".dropdown-content")
        dropdowns.forEach((dropdown) => {
            if (!dropdown.classList.contains("hidden")) {
                dropdown.classList.add("hidden")
            }
        })
    })

    // FAQ accordion functionality
    const faqQuestions = document.querySelectorAll(".faq-question")
    if (faqQuestions.length > 0) {
        faqQuestions.forEach((question) => {
            question.addEventListener("click", function () {
                const answer = this.querySelector(".faq-answer")
                const arrow = this.querySelector("i.ri-arrow-down-s-line")

                if (answer && arrow) {
                    answer.classList.toggle("hidden")
                    arrow.classList.toggle("rotate-180")
                }
            })
        })
    }

    // Select all checkboxes
    const selectAllCheckbox = document.getElementById("selectAll")
    if (selectAllCheckbox) {
        selectAllCheckbox.addEventListener("change", function () {
            const checkboxes = document.querySelectorAll(".faq-checkbox, .survey-checkbox, .user-checkbox")
            checkboxes.forEach((checkbox) => {
                checkbox.checked = this.checked
            })
        })
    }

    // Modal functionality
    const modalTriggers = document.querySelectorAll("[data-modal-toggle]")
    modalTriggers.forEach((trigger) => {
        const targetId = trigger.getAttribute("data-modal-toggle")
        const target = document.getElementById(targetId)

        if (target) {
            trigger.addEventListener("click", () => {
                target.classList.toggle("hidden")
            })

            // Close button
            const closeButtons = target.querySelectorAll("[data-modal-close]")
            closeButtons.forEach((button) => {
                button.addEventListener("click", () => {
                    target.classList.add("hidden")
                })
            })

            // Close on backdrop click
            target.addEventListener("click", (e) => {
                if (e.target === target) {
                    target.classList.add("hidden")
                }
            })
        }
    })
})

// Function to initialize charts
function initializeChart(chartId) {
    // Check if echarts is defined before using it
    if (typeof echarts === "undefined") {
        console.error("ECharts is not defined. Make sure it is properly imported.")
        return // Exit the function if echarts is not available
    }

    const chart = echarts.init(document.getElementById(chartId))

    // Default options
    let options = {}

    // Customize based on chart ID
    switch (chartId) {
        case "participationChart":
            options = {
                tooltip: {
                    trigger: "axis",
                    backgroundColor: "rgba(255, 255, 255, 0.8)",
                    borderColor: "#e5e7eb",
                    textStyle: {
                        color: "#1f2937",
                    },
                },
                legend: {
                    data: ["Students", "Faculty", "Staff"],
                    bottom: 0,
                },
                grid: {
                    left: "3%",
                    right: "4%",
                    bottom: "15%",
                    top: "3%",
                    containLabel: true,
                },
                xAxis: {
                    type: "category",
                    boundaryGap: false,
                    data: ["Jan", "Feb", "Mar", "Apr", "May"],
                },
                yAxis: {
                    type: "value",
                    axisLine: {
                        show: false,
                    },
                    splitLine: {
                        lineStyle: {
                            color: "#f3f4f6",
                        },
                    },
                },
                series: [
                    {
                        name: "Students",
                        type: "line",
                        smooth: true,
                        lineStyle: {
                            width: 3,
                            color: "rgba(87, 181, 231, 1)",
                        },
                        areaStyle: {
                            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                                {
                                    offset: 0,
                                    color: "rgba(87, 181, 231, 0.2)",
                                },
                                {
                                    offset: 1,
                                    color: "rgba(87, 181, 231, 0.05)",
                                },
                            ]),
                        },
                        symbol: "none",
                        data: [320, 432, 501, 634, 890],
                    },
                    {
                        name: "Faculty",
                        type: "line",
                        smooth: true,
                        lineStyle: {
                            width: 3,
                            color: "rgba(141, 211, 199, 1)",
                        },
                        areaStyle: {
                            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                                {
                                    offset: 0,
                                    color: "rgba(141, 211, 199, 0.2)",
                                },
                                {
                                    offset: 1,
                                    color: "rgba(141, 211, 199, 0.05)",
                                },
                            ]),
                        },
                        symbol: "none",
                        data: [120, 182, 191, 234, 290],
                    },
                    {
                        name: "Staff",
                        type: "line",
                        smooth: true,
                        lineStyle: {
                            width: 3,
                            color: "rgba(251, 191, 114, 1)",
                        },
                        areaStyle: {
                            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                                {
                                    offset: 0,
                                    color: "rgba(251, 191, 114, 0.2)",
                                },
                                {
                                    offset: 1,
                                    color: "rgba(251, 191, 114, 0.05)",
                                },
                            ]),
                        },
                        symbol: "none",
                        data: [150, 232, 201, 154, 190],
                    },
                ],
            }
            break

        case "userDistributionChart":
            options = {
                tooltip: {
                    trigger: "item",
                    formatter: "{a} <br/>{b}: {c} ({d}%)",
                },
                legend: {
                    orient: "vertical",
                    right: 10,
                    top: "center",
                    data: ["Students", "Faculty", "Staff", "Admins"],
                },
                series: [
                    {
                        name: "User Distribution",
                        type: "pie",
                        radius: ["50%", "70%"],
                        avoidLabelOverlap: false,
                        label: {
                            show: false,
                            position: "center",
                        },
                        emphasis: {
                            label: {
                                show: true,
                                fontSize: "16",
                                fontWeight: "bold",
                            },
                        },
                        labelLine: {
                            show: false,
                        },
                        data: [
                            { value: 735, name: "Students", itemStyle: { color: "#3b82f6" } },
                            { value: 310, name: "Faculty", itemStyle: { color: "#10b981" } },
                            { value: 234, name: "Staff", itemStyle: { color: "#f59e0b" } },
                            { value: 15, name: "Admins", itemStyle: { color: "#ef4444" } },
                        ],
                    },
                ],
            }
            break

        case "categoryChart":
            options = {
                tooltip: {
                    trigger: "item",
                    formatter: "{a} <br/>{b}: {c} ({d}%)",
                },
                series: [
                    {
                        name: "Categories",
                        type: "pie",
                        radius: ["40%", "70%"],
                        avoidLabelOverlap: false,
                        label: {
                            show: false,
                        },
                        emphasis: {
                            label: {
                                show: false,
                            },
                        },
                        labelLine: {
                            show: false,
                        },
                        data: [
                            { value: 8, name: "Recycling", itemStyle: { color: "#3b82f6" } },
                            { value: 5, name: "Energy", itemStyle: { color: "#10b981" } },
                            { value: 4, name: "Water", itemStyle: { color: "#f59e0b" } },
                            { value: 3, name: "Transport", itemStyle: { color: "#8b5cf6" } },
                            { value: 2, name: "Food", itemStyle: { color: "#ef4444" } },
                            { value: 2, name: "Innovation", itemStyle: { color: "#6366f1" } },
                        ],
                    },
                ],
            }
            break

        case "deviceChart":
            options = {
                tooltip: {
                    trigger: "item",
                    formatter: "{a} <br/>{b}: {c} ({d}%)",
                },
                series: [
                    {
                        name: "Device Usage",
                        type: "pie",
                        radius: "70%",
                        data: [
                            { value: 58, name: "Mobile", itemStyle: { color: "#3b82f6" } },
                            { value: 32, name: "Desktop", itemStyle: { color: "#10b981" } },
                            { value: 10, name: "Tablet", itemStyle: { color: "#f59e0b" } },
                        ],
                        emphasis: {
                            itemStyle: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: "rgba(0, 0, 0, 0.5)",
                            },
                        },
                        label: {
                            formatter: "{b}: {d}%",
                        },
                    },
                ],
            }
            break

        case "roleChart":
        case "departmentChart":
            // Similar to other pie charts
            options = {
                tooltip: {
                    trigger: "item",
                    formatter: "{a} <br/>{b}: {c} ({d}%)",
                },
                series: [
                    {
                        name: chartId === "roleChart" ? "Roles" : "Departments",
                        type: "pie",
                        radius: "70%",
                        data:
                            chartId === "roleChart"
                                ? [
                                    { value: 735, name: "Students", itemStyle: { color: "#3b82f6" } },
                                    { value: 310, name: "Faculty", itemStyle: { color: "#10b981" } },
                                    { value: 234, name: "Staff", itemStyle: { color: "#f59e0b" } },
                                    { value: 15, name: "Admins", itemStyle: { color: "#ef4444" } },
                                ]
                                : [
                                    { value: 335, name: "Science", itemStyle: { color: "#10b981" } },
                                    { value: 310, name: "Engineering", itemStyle: { color: "#3b82f6" } },
                                    { value: 234, name: "Arts", itemStyle: { color: "#8b5cf6" } },
                                    { value: 135, name: "Business", itemStyle: { color: "#f59e0b" } },
                                    { value: 154, name: "Medicine", itemStyle: { color: "#ef4444" } },
                                    { value: 335, name: "Others", itemStyle: { color: "#6b7280" } },
                                ],
                        emphasis: {
                            itemStyle: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: "rgba(0, 0, 0, 0.5)",
                            },
                        },
                        label: {
                            formatter: "{b}: {d}%",
                        },
                    },
                ],
            }
            break

        default:
            // Default bar chart
            options = {
                tooltip: {
                    trigger: "axis",
                    axisPointer: {
                        type: "shadow",
                    },
                },
                grid: {
                    left: "3%",
                    right: "4%",
                    bottom: "3%",
                    containLabel: true,
                },
                xAxis: [
                    {
                        type: "category",
                        data: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                        axisTick: {
                            alignWithLabel: true,
                        },
                    },
                ],
                yAxis: [
                    {
                        type: "value",
                    },
                ],
                series: [
                    {
                        name: "Data",
                        type: "bar",
                        barWidth: "60%",
                        data: [10, 52, 200, 334, 390, 330, 220],
                        itemStyle: {
                            color: "#10b981",
                        },
                    },
                ],
            }
    }

    // Set chart options and render
    chart.setOption(options)

    // Handle window resize
    window.addEventListener("resize", () => {
        chart.resize()
    })
}
