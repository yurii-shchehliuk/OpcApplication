import { AfterViewInit, Component, OnInit } from '@angular/core';
import { MonitorAction, NewNode, NodeData } from '../models/models';
import { ChartDataset, ChartConfiguration, Chart, ChartData } from 'chart.js';
import { AppService } from '../app.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-channel',
  templateUrl: './channel.component.html',
  styleUrls: ['./channel.component.scss'],
})
export class ChannelComponent implements OnInit, AfterViewInit {
  chart: Chart;
  nodeArray: NodeData[] = [];
  datasets: ChartDataset[] = [];
  labels: string[] = [];
  channelSource = '';

  subscriptions: Subscription[] = [];

  newNode: NewNode = {
    nodeId: '',
    name: '',
    mSecs: null,
    range: null,
    Action: MonitorAction.Monitor,
    group: this.channelSource,
  };

  constructor(private appService: AppService) {}

  ngOnInit() {
    this.createChart();

    // listen from selected channel
    // signalr connection is created in channel-monitor
    let sub = this.appService.getChannel.subscribe((data: string) => {
      this.channelSource = data;

      this.nodeArray = [];
      this.datasets = [];
      this.labels = [];

      this.appService.getNodesWeb();

      this.chart.destroy();
      this.createChart();
    });

    // Fill up chart with data if not in ignore and if
    let sub2 = this.appService.getNodeObservable.subscribe((nodeData) => {
      if (
        this.nodeArray.findIndex((c) => c.nodeId === nodeData.nodeId) === -1
      ) {
        this.nodeArray.push(nodeData);
      }
      this.pushEventToChartData(nodeData);
    });

    let sub3 = this.appService.getNodeConfigurationsObservable.subscribe(
      (nodeDataArr) => {
        nodeDataArr.map((nodeData) => {
          if (
            this.nodeArray.findIndex((c) => c.nodeId === nodeData.nodeId) === -1
          ) {
            this.nodeArray.push(nodeData);
          }
          this.pushEventToChartData(nodeData);
        });
      }
    );

    //subscriptions
    this.subscriptions.push(sub);
    this.subscriptions.push(sub2);
  }

  ngAfterViewInit(): void {
    this.refreshChart();
  }

  refreshChart(event: any = undefined) {
    this.appService.signalrInit();
    if (event) console.log(this.datasets);

    this.chart.render();
    this.chart.update();
  }

  nodeMonitor() {
    this.newNode.group = this.channelSource;
    this.newNode.Action = MonitorAction.Monitor;

    this.appService.nodeMonitorWeb(this.newNode);
    this.newNode = (<Partial<NewNode>>{}) as NewNode;
  }

  removeNode(nodeId: any) {
    nodeId.Action = MonitorAction.Unmomonitor;
    this.appService.nodeMonitorWeb(nodeId);

    let index = this.datasets.findIndex((c) => c.label === nodeId);
    this.datasets.splice(index, 1);
    this.nodeArray.splice(index, 1);
    this.refreshChart();
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
    let dsetItem = this.datasets.findIndex((c) => c.label === event.nodeId);

    if (dsetItem === -1) {
      const newDataset: ChartDataset = {
        label: event.nodeId,
        data: [event.value],

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
      this.datasets[dsetItem].data.push(event.value);
    }

    if (this.labels.findIndex((c) => c === event.storeTime) === -1) {
      this.labels.push(event.storeTime);
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
