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
export class ChannelsManagerComponent implements OnInit {
  channelsArr: any[] = [];

  @ViewChild('channelList') channelArrElem: ElementRef;

  constructor(private appService: AppService) {
    this.appService.signalrInit();
  }

  ngOnInit(): void {
    this.appService.getChannels.subscribe((data) => {
      this.channelsArr = data;
    });
  }

  selectChannel(channel: string) {
    this.appService.signalReset();

    let childrens = this.channelArrElem.nativeElement.children;
    for (let item of childrens) {
      item.classList.remove('channel-selected');
      let value = item.getElementsByClassName('channel-text')[0].innerText;

      if (value === channel) {
        item.classList.add('channel-selected');
        this.appService.joinNewGroup(channel);
      }
    }
  }

  addGroup(nameElem: any) {
    let name = nameElem.value;
    if (name.length > 0) this.channelsArr.push({ Name: name });
    nameElem.value = '';
    this.appService.addChannelWeb(name);
  }

  removeGroup(name: string) {
    const index = this.channelsArr.indexOf(name);
    const x = this.channelsArr.splice(index, 1);
    this.appService.removeChannelWeb(name);
  }
}
