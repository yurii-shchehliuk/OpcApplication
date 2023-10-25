import { Component } from '@angular/core';
import { Chart, ChartDataset, ChartConfiguration } from 'chart.js';
import { Subscription } from 'rxjs';
import { NodeValue, NodeReference } from '../models/nodeModels';
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
  nodeArray: NodeReference[] = [];
  datasets: ChartDataset[] = [];
  labels: string[] = [];
  sessionSource = '';

  subscriptions: Subscription[] = [];
  newNode: NodeReference = (<Partial<NodeReference>>{}) as NodeReference;

  constructor(
    private sessionService: SessionService,
    private subscriptionService: SubscriptionService,
    private nodeService: NodeService,
    private communicationService: CommunicationService,
    public dialog: MatDialog
  ) {}

  ngOnInit() {
    this.createChart();
    // listen from selected session
    // signalr connection is created in session-monitor
    this.sessionService.getSession.subscribe((data: string) => {
      this.sessionSource = data;

      this.nodeArray = [];
      this.datasets = [];
      this.labels = [];

      this.nodeService.configNodes$.subscribe(
        (nodeRefList: NodeReference[]) => {
          this.nodeArray = [];

          nodeRefList.map((NodeValue: NodeReference) => {
            this.nodeArray.push(NodeValue);
          });
        }
      );

      this.chart.destroy();
      this.createChart();
    });

    // Fill up chart with data if not present ignore
    this.communicationService.getNodeObservable.subscribe(
      (NodeValue: NodeValue) => {
        this.pushEventToChartData(NodeValue);
      }
    );

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

  subscribeToNode(event: any, node: NodeReference) {
    if (node.subscriptionId) {
      this.subscriptionService
        .deleteSubs(node.subscriptionId, node.nodeId)
        .subscribe(() => {
          let oldNodeRef = this.nodeArray.findIndex(
            (c) => c.nodeId === node.nodeId
          );
          this.nodeArray[oldNodeRef].subscriptionId = null;
        });
    } else {
      this.subscriptionService.createSubs(node).subscribe((res) => {
        let oldNodeRef = this.nodeArray.findIndex(
          (c) => c.nodeId === node.nodeId
        );
        this.nodeArray[oldNodeRef] = res;
      });
    }
  }

  addConfigNode() {
    this.nodeService.addConfigNode(this.newNode);
    this.newNode = (<Partial<NodeReference>>{}) as NodeReference;
  }

  removeNode(node: NodeReference) {
    this.subscriptionService
      .deleteSubs(node.subscriptionId!, node.nodeId)
      .subscribe();
    this.nodeService.deleteConfigNode(node.nodeId);
    this.refreshChart();
  }

  hideNode(event: any, nodeId: string) {
    // this.sessionService.nodeMonitor(this.newNode, this.sessionSource, 'unmonitor');

    let dataset = this.datasets.find((c) => c.label === nodeId);
    if (!dataset) return;

    //hide
    dataset.hidden = !dataset.hidden;
    this.chart.update();

    //add background
    event.target.parentElement.parentElement.classList.toggle(
      'background-hidden'
    );
    //chnage icon
    event.target.classList.toggle('bi-eye');
    event.target.classList.toggle('bi-eye-slash');

    event.stopPropagation();
    event.preventDefault();
  }

  getSubscriptionIcon(item: NodeReference): string {
    if (item.subscriptionId && this.getNodeValue(item.nodeId) != 0) {
      return 'bi-send-dash';
    }
    return 'bi-send-plus';
  }

  populateNode(node: NodeReference) {
    this.newNode = JSON.parse(JSON.stringify(node));
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

  private pushEventToChartData(event: NodeValue): void {
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
