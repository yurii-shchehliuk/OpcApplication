import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { AppService } from '../app.service';

@Component({
  selector: 'app-channels-manager',
  templateUrl: './channels-manager.component.html',
  styleUrls: ['./channels-manager.component.scss'],
})
export class ChannelsManagerComponent implements AfterViewInit {
  channelsArr: string[] = ['All', 'Group1', 'Group2'];

  @ViewChild('channelList') channelArrElem: ElementRef;

  constructor(private appService: AppService) {}

  ngAfterViewInit(): void {
    this.selectChannel(this.channelsArr[0]);
  }

  selectChannel(channel: string) {
    this.appService.signalReset();
    let childrens = this.channelArrElem.nativeElement.children;
    for (let item of childrens) {
      item.classList.remove('channel-selected');

      let value = item.getElementsByClassName('channel-text')[0].innerText;
      if (channel === this.channelsArr[0] && channel === value) {
        this.appService.joinPublic();
        item.classList.add('channel-selected');
      } else if (channel === value && channel !== this.channelsArr[0]) {
        item.classList.add('channel-selected');
        this.appService.joinNewGroup(channel);
      }
    }
  }

  addGroup(nameElem: any) {
    let name = nameElem.value;
    if (name.length > 0) this.channelsArr.push(name);
    nameElem.value = '';
  }

  removeGroup(name: string) {
    const index = this.channelsArr.indexOf(name);
    const x = this.channelsArr.splice(index, 1);
  }
}
