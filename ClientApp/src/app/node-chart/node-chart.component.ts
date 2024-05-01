import { Component } from '@angular/core';
import {
  Chart,
  ChartDataset,
  ChartConfiguration,
  registerables,
} from 'chart.js';
import { Subscription, takeUntil } from 'rxjs';
import { SessionEntity } from '../models/sessionModels';
import { SessionAccessorService } from '../shared/session-accessor.service';
import {
  MonitoredItemConfig,
  MonitoredItemValue,
} from '../models/monitoredItem';
import { SessionState } from '../models/enums';
import annotationPlugin from 'chartjs-plugin-annotation';
import { SharedService } from '../shared/shared.service';
import { SubscriptionConfig } from '../models/subscriptionModels';
import { CommunicationService } from '../services/communication.service';
import { MonitoredItemService } from '../services/monitored-item.service';
import { BaseComponent } from '../shared/components/base/base.component';

@Component({
  selector: 'app-node-chart',
  templateUrl: './node-chart.component.html',
  styleUrls: ['./node-chart.component.scss'],
})
export class NodeChartComponent extends BaseComponent {
  chart: Chart;
  datasets: ChartDataset[] = [];
  labels: string[] = [];
  minLineValue: number = 0;
  maxLineValue: number = 0;
  sessionSource: SessionEntity = (<Partial<SessionEntity>>{}) as SessionEntity;
  sessionState = SessionState;
  nodeArray: MonitoredItemConfig[] = [];

  constructor(
    private sessionAccessor: SessionAccessorService,
    private monitoredItemService: MonitoredItemService,
    private sharedService: SharedService,
    private communicationService: CommunicationService
  ) {
    super();
    Chart.register(...registerables, annotationPlugin);
  }

  ngOnInit() {
    this.createChart();
    // when session changed - reset everything
    this.sessionAccessor.currentSession$
      .pipe(takeUntil(this.destroy$))
      .subscribe((data: SessionEntity) => {
        this.sessionSource = data;

        this.nodeArray = [];
        this.datasets = [];
        this.labels = [];

        this.chart.destroy();
        this.createChart();
      });

    // Fill up chart with data if not present ignore
    this.communicationService.getNodeObservable
      .pipe(takeUntil(this.destroy$))
      .subscribe((NodeValue: MonitoredItemValue) => {
        this.pushEventToChartData(NodeValue);
      });

    // Remove node from the subscription then from the dataset
    this.monitoredItemService.nodeToRemove$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (nodeId: string) => {
          let datasetIndex = this.datasets.findIndex((c) => c.label === nodeId);
          this.datasets.splice(datasetIndex, 1);
          this.refreshChart();
        },
      });

    //on subscriptionConfig modify refresh chart
    this.sharedService.chartValueBoundaries$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (value: SubscriptionConfig) => {
          this.minLineValue = value.minValue;
          this.maxLineValue = value.maxValue;
          this.updateChart();
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

  private updateChart() {
    if (this.minLineValue == undefined) {
      return;
    }

    if (this.minLineValue === 0 && this.maxLineValue === 0) {
      this.chart.options.plugins!.annotation!.annotations = {};
      this.chart.update();
      return;
    }

    this.updateBackgroundColor();
    this.updateAnnotations();
    this.chart.update();
  }

  private updateBackgroundColor() {
    const dataset = this.datasets[0];
    if (dataset === undefined) return;
    dataset.backgroundColor = dataset.data.map((value: any) => {
      if (value < this.minLineValue || value > this.maxLineValue) {
        // Color for values outside the range
        return 'rgba(255, 99, 132, 0.2)';
      }
      // Color for values inside the range
      return 'rgba(75, 192, 192, 0.2)';
    });
  }

  private updateAnnotations() {
    this.chart.options.plugins!.annotation!.annotations = {
      minLine: {
        type: 'line',
        yMin: this.minLineValue,
        yMax: this.minLineValue,
        borderColor: 'green',
        borderWidth: 2,
        label: {
          content: 'Min Line',
          position: 'start',
        },
      },
      maxLine: {
        type: 'line',
        yMin: this.maxLineValue,
        yMax: this.maxLineValue,
        borderColor: 'red',
        borderWidth: 2,
        label: {
          content: 'Max Line',
          position: 'start',
        },
      },
    };
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

  private pushEventToChartData(event: MonitoredItemValue): void {
    let dsetItem = this.datasets.findIndex(
      (c) => c.label === event.startNodeId
    );

    if (dsetItem === -1) {
      const newDataset: ChartDataset = {
        label: event.startNodeId,
        data: [event.value],
        borderColor: this.getRandomColor(),
      };

      this.datasets.push(newDataset);
    } else {
      this.removeLastElementFromChartDataAndLabel(this.datasets[dsetItem], 12);
      this.datasets[dsetItem].data.push(event.value);
    }

    if (this.labels.findIndex((c) => c === event.createdAt) === -1) {
      this.labels.push(event.createdAt);
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
}
