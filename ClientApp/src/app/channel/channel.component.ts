import { Component } from '@angular/core';
import { MonitorAction, NewNode, NodeData } from '../models/models';
import { ChartDataset, ChartConfiguration, Chart, ChartData } from 'chart.js';
import { AppService } from '../app.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-channel',
  templateUrl: './channel.component.html',
  styleUrls: ['./channel.component.scss'],
})
export class ChannelComponent {
  chart: Chart;
  nodeArray: NodeData[] = [];
  ignoreArray: string[] = [];
  datasets: ChartDataset[] = [];
  labels: string[] = [];
  channelSource = '';

  subscriptions: Subscription[] = [];

  newNode: NewNode = {
    NodeId: '',
    Name: '',
    MSecs: null,
    Range: null,
    Action: MonitorAction.Monitor,
    Group: this.channelSource,
  };

  constructor(private appService: AppService) {}

  ngOnInit() {
    this.createChart();

    //listen from selected channel
    let sub = this.appService.getChannel().subscribe((data: string) => {
      this.channelSource = data;

      this.ignoreArray = [];
      this.nodeArray = [];
      this.datasets = [];
      this.labels = [];

      this.chart.destroy();
      this.createChart();
      this.refreshChart();
    });

    // Fill up chart with data if not in ignore and if
    let sub2 = this.appService.getMessageObservable().subscribe((nodeData) => {
      if (this.ignoreArray.findIndex((c) => c === nodeData.NodeId) !== -1)
        return;
      if (
        this.nodeArray.findIndex((c) => c.NodeId === nodeData.NodeId) === -1
      ) {
        this.nodeArray.push(nodeData);
      }
      this.pushEventToChartData(nodeData);
    });

    //subscriptions
    this.subscriptions.push(sub);
    this.subscriptions.push(sub2);
  }

  refreshChart(event: any = undefined) {
    if (event) console.log(this.datasets);

    this.chart.render();
    this.chart.update();

    this.appService.askForGraph();
  }

  nodeMonitor() {
    let index = this.datasets.findIndex((c) => c.label === this.newNode.NodeId);
    this.ignoreArray.splice(index, 1);

    this.appService.nodeMonitor(this.newNode);
    this.newNode = (<Partial<NewNode>>{}) as NewNode;
  }

  hideNode(event: any, nodeId: string) {
    // this.appService.nodeMonitor(this.newNode, this.channelSource, 'unmonitor');

    let dataset = this.datasets.find((c) => c.label === nodeId);
    if (!dataset) return;

    //hide
    dataset.hidden = !dataset.hidden;
    this.chart.update();
    //change text
    // dataset.hidden
    //   ? (event.target.innerHTML = 'Show')
    //   : (event.target.innerHTML = 'Hide');

    //add background
    event.target.parentElement.parentElement.classList.toggle(
      'background-hidden'
    );
    event.target.classList.toggle('bi-eye');
    event.target.classList.toggle('bi-eye-slash');

    event.stopPropagation();
    event.preventDefault();
  }

  removeNode(nodeId: string) {
    let index = this.datasets.findIndex((c) => c.label === nodeId);
    this.datasets.splice(index, 1);
    this.nodeArray.splice(index, 1);
    this.ignoreArray.push(nodeId);
    this.refreshChart();
  }

  protected getNodeValue(nodeId: string) {
    let dataset = this.datasets.find((c) => c.label === nodeId);
    if (!dataset) return 0;

    return dataset?.data[dataset?.data.length - 1];
  }

  private createChart(): void {
    const ctx = document.getElementById('myChart') as HTMLCanvasElement;
    let self = this;
    const chartCfg: ChartConfiguration = {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: this.datasets,
      },
      options: {
        responsive: true,
        scales: {
          x: {
            ticks: {
              callback: function (value, index, values) {
                return self.labels[index]; // Display labels instead of values
              },
            },
            // type: 'time',
            display: true,
            title: {
              display: true,
              text: 'Time',
            },
          },
          y: {
            display: true,
            title: {
              display: true,
              text: 'Temperature',
            },
          },
        },
      },
    };
    this.chart = new Chart(ctx, chartCfg);
  }

  private pushEventToChartData(event: NodeData): void {
    let dsetItem = this.datasets.findIndex((c) => c.label === event.NodeId);

    if (dsetItem === -1) {
      const newDataset: ChartDataset = {
        label: event.NodeId,
        data: [event.Value],

        // data: [
        //   {
        //     x: new Date(event.StoreTime),
        //     y: event.Value,
        //   },
        // ],
        borderColor: this.getRandomColor(),
      };

      this.datasets.push(newDataset);
    } else {
      this.removeLastElementFromChartDataAndLabel(this.datasets[dsetItem], 12);
      this.datasets[dsetItem].data.push(event.Value);
    }

    if (this.labels.findIndex((c) => c === event.StoreTime) === -1) {
      this.labels.push(event.StoreTime);
    }

    //TODO: the problem is that labels are adding as a stack and chart is with step one
    this.chart.update();
  }

  private getRandomColor(): string {
    return `#${Math.floor(Math.random() * 16777215).toString(16)}`;
  }

  private removeLastElementFromChartDataAndLabel(
    chartItem: ChartDataset,
    limit: number
  ): void {
    ///todo: remove old record from dataset
    if (chartItem.data.length >= limit) {
      chartItem.data.shift();
    }

    ///todo: remove old record from labels
    if (this.labels.length >= limit) {
      this.labels.shift();
    }

    this.refreshChart();
  }

  ngOnDestroy() {
    this.subscriptions.forEach((c) => c.unsubscribe());
  }
}
