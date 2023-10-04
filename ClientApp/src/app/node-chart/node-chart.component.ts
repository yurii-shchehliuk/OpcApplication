import { Component } from '@angular/core';
import { Chart, ChartDataset, ChartConfiguration } from 'chart.js';
import { Subscription } from 'rxjs';
import { NodeData } from '../models/nodeModels';
import { CommunicationService } from '../services/communication.service';
import { NodeService } from '../services/node.service';
import { SessionService } from '../services/session.service';
import { SubscriptionService } from '../services/subscription.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-node-chart',
  templateUrl: './node-chart.component.html',
  styleUrls: ['./node-chart.component.scss'],
})
export class NodeChartComponent {
  chart: Chart;
  nodeArray: any[] = [];
  datasets: ChartDataset[] = [];
  labels: string[] = [];
  channelSource = '';

  subscriptions: Subscription[] = [];

  newNode: any = {
    nodeId: '',
    browseName: '',
    displayName: '',
    nodeClass: 0,
    sessionName: '',
    mSecs: null,
    range: null,
  };

  constructor(
    private sessionService: SessionService,
    private subscriptionService: SubscriptionService,
    private nodeService: NodeService,
    private communicationService: CommunicationService,
    public dialog: MatDialog
  ) {}

  ngOnInit() {
    this.createChart();
    // listen from selected channel
    // signalr connection is created in channel-monitor
    this.nodeService.getConfigNodes();
    this.sessionService.getChannel.subscribe((data: string) => {
      this.channelSource = data;

      this.nodeArray = [];
      this.datasets = [];
      this.labels = [];

      this.nodeService.configNodes$.subscribe((nodeDataArr: any) => {
        this.nodeArray = [];

        nodeDataArr.map((nodeData: any) => {
          this.nodeArray.push(nodeData);
          this.pushEventToChartData(nodeData);
        });
      });

      this.chart.destroy();
      this.createChart();
    });

    // Fill up chart with data if not present ignore
    this.communicationService.getNodeObservable.subscribe((nodeData) => {
      if (
        this.nodeArray.findIndex((c) => c.nodeId === nodeData.nodeId) === -1
      ) {
        this.nodeArray.push(nodeData);
      }
      this.pushEventToChartData(nodeData);
    });

    //subscriptions
  }

  ngAfterViewInit(): void {
    this.refreshChart();
  }

  refreshChart(event: any = undefined) {
    if (event) console.log(this.datasets);

    this.chart.render();
    this.chart.update();
  }

  nodeMonitor() {
    this.newNode.sessionName = this.channelSource;

    this.subscriptionService.createSubs(this.newNode);
  }

  removeNode(nodeId: string) {
    this.subscriptionService.deleteSubs(nodeId);
    this.nodeService.deleteConfigNode(nodeId);
    this.refreshChart();
  }

  hideNode(event: any, nodeId: string) {
    // this.sessionService.nodeMonitor(this.newNode, this.channelSource, 'unmonitor');

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
