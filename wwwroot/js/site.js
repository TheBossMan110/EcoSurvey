// Common JavaScript functions for EcoSurvey

// Import echarts (assuming it's available as a module)
// If not using modules, ensure echarts is included via a <script> tag in your HTML
// For example: <script src="https://cdn.jsdelivr.net/npm/echarts@5.4.2/dist/echarts.min.js"></script>
// If you're using a bundler like Webpack or Parcel, you can import it like this:
// import * as echarts from 'echarts';

// Initialize charts when the DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    // Initialize participation chart if the element exists
    const participationChartElement = document.getElementById("participationChart")
    if (participationChartElement) {
        const participationChart = echarts.init(participationChartElement)

        const option = {
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
            xAxis: {
                type: "category",
                data: ["Jan", "Feb", "Mar", "Apr", "May"],
                axisTick: {
                    alignWithLabel: true,
                },
            },
            yAxis: {
                type: "value",
            },
            series: [
                {
                    name: "Participants",
                    type: "bar",
                    barWidth: "60%",
                    data: [120, 200, 150, 180, 230],
                    itemStyle: {
                        color: "#2E7D32",
                    },
                },
            ],
        }

        participationChart.setOption(option)

        // Handle resize
        window.addEventListener("resize", () => {
            participationChart.resize()
        })
    }

    // Initialize survey completion chart if the element exists
    const surveyCompletionElement = document.getElementById("surveyCompletionChart")
    if (surveyCompletionElement) {
        const surveyCompletionChart = echarts.init(surveyCompletionElement)

        const option = {
            tooltip: {
                trigger: "item",
            },
            legend: {
                orient: "vertical",
                left: "left",
                textStyle: {
                    color: "#6B7280",
                },
            },
            series: [
                {
                    name: "Survey Status",
                    type: "pie",
                    radius: "70%",
                    data: [
                        { value: 12, name: "Completed", itemStyle: { color: "#2E7D32" } },
                        { value: 8, name: "In Progress", itemStyle: { color: "#F59E0B" } },
                        { value: 5, name: "Not Started", itemStyle: { color: "#6B7280" } },
                    ],
                    emphasis: {
                        itemStyle: {
                            shadowBlur: 10,
                            shadowOffsetX: 0,
                            shadowColor: "rgba(0, 0, 0, 0.5)",
                        },
                    },
                },
            ],
        }

        surveyCompletionChart.setOption(option)

        // Handle resize
        window.addEventListener("resize", () => {
            surveyCompletionChart.resize()
        })
    }
})

// Mobile menu toggle
function toggleMobileMenu() {
    const mobileMenu = document.getElementById("mobileMenu")
    if (mobileMenu) {
        mobileMenu.classList.toggle("hidden")
    }
}
