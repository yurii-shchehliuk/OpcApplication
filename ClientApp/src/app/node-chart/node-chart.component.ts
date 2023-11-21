import { Component } from '@angular/core';
import { Chart, ChartDataset, ChartConfiguration } from 'chart.js';
import { Subscription } from 'rxjs';
import { NodeValue, NodeReference } from '../models/nodeModels';
import { NodeService } from '../services/node.service';
import { SubscriptionService } from '../services/subscription.service';
import { MatDialog } from '@angular/material/dialog';
import { SessionEntity } from '../models/sessionModels';
import { SessionState } from 'src/app/models/sessionModels';
import { SessionAccessorService } from '../shared/session-accessor.service';
import { SubscriptionParametersDialogComponent } from '../subscription/subscription-parameters-dialog/subscription-parameters-dialog.component';

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
  sessionSource: SessionEntity = (<Partial<SessionEntity>>{}) as SessionEntity;
  sessionState = SessionState;
  subscriptions: Subscription[] = [];

  constructor(
    private sessionAccessor: SessionAccessorService,
    private nodeService: NodeService
  ) {}

  ngOnInit() {
    this.createChart();
    // when session changed - reset everything
    this.sessionAccessor.currentSession$.subscribe((data: SessionEntity) => {
      this.sessionSource = data;

      this.nodeArray = [];
      this.datasets = [];
      this.labels = [];

      this.chart.destroy();
      this.createChart();
    });

    // Fill up chart with data if not present ignore
    this.nodeService.getNodeValueObservable$.subscribe(
      (NodeValue: NodeValue) => {
        this.pushEventToChartData(NodeValue);
      }
    );

    this.nodeService.nodeToRemove$.subscribe({
      next: (nodeId: string) => {
        let datasetIndex = this.datasets.findIndex(
          (c) => c.label === nodeId
        );
        this.datasets.splice(datasetIndex, 1);
        this.refreshChart();
      },
    });
  }

  ngAfterViewInit(): void {
    this.refreshChart();
  }

  refreshChart(event: any = undefined) {
    if (event) console.log(this.datasets);

    this.chart.render();
    this.chart.update();
  }

  hideNode(nodeId: string) {
    let dataset = this.datasets.find((c) => c.label === nodeId);
    if (!dataset) return;

    //hide
    dataset.hidden = !dataset.hidden;
    this.chart.update();
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
