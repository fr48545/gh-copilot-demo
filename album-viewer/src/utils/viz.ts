import * as d3 from "d3";

export function loadDataAndRenderChart(
  jsonFilePath: string,
  containerSelector: string
): Promise<void> {
  return d3.json(jsonFilePath).then((data: any) => {
    if (!data) {
      console.error("No data loaded from", jsonFilePath);
      return;
    }

    const width = 600;
    const height = 400;
    const margin = { top: 20, right: 20, bottom: 30, left: 40 };

    const svg = d3
      .select(containerSelector)
      .append("svg")
      .attr("width", width)
      .attr("height", height)
      .attr("viewBox", `0 0 ${width} ${height}`);

    // Create scales for the x and y axes.
    const x = d3
      .scaleBand()
      .domain(data.map((d: any) => d.label))
      .range([margin.left, width - margin.right])
      .padding(0.1);

    const y = d3
      .scaleLinear()
      .domain([0, d3.max(data, (d: any) => d.value) as number])
      .nice()
      .range([height - margin.bottom, margin.top]);

    const line = d3
      .line<any>()
      .x((d) => (x(d.label) as number) + x.bandwidth() / 2)
      .y((d) => y(d.value));

    svg
      .append("path")
      .datum(data)
      .attr("fill", "none")
      .attr("stroke", "steelblue")
      .attr("stroke-width", 2)
      .attr("d", line);

    svg
      .append("g")
      .attr("transform", `translate(0,${height - margin.bottom})`)
      .call(d3.axisBottom(x));

    svg
      .append("g")
      .attr("transform", `translate(${margin.left},0)`)
      .call(d3.axisLeft(y));
  });
}


