import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  NgbDropdown,
  NgbDropdownItem,
  NgbDropdownMenu,
  NgbDropdownToggle,
  NgbModal,
} from '@ng-bootstrap/ng-bootstrap';
import { AsyncPipe } from '@angular/common';
import { Client, ClientService } from '../../../core/services/client.service';
import { CreateClientModal } from '../../../core/components/create-client-modal/create-client-modal';

@Component({
  selector: 'app-client-selector',
  imports: [NgbDropdownItem, AsyncPipe, NgbDropdownMenu, NgbDropdown, NgbDropdownToggle],
  templateUrl: './client-selector.html',
})
export class ClientSelector {
  @Input()
  public selected: Client | null = null;
  @Input()
  public placeholder = 'All clients';
  @Output()
  public selectedChange = new EventEmitter<Client | null>();

  constructor(
    protected clientService: ClientService,
    private _modal: NgbModal,
  ) {}

  protected selectClient(client: Client | null): void {
    this.selected = client;
    this.selectedChange.emit(client);
  }

  protected onDropdownScroll(event: Event): void {
    const target = event.target as HTMLElement;
    const nearBottom = target.scrollHeight - target.scrollTop <= target.clientHeight + 50;

    if (nearBottom) {
      this.clientService.loadNextPage();
    }
  }

  protected openCreateClientModal(): void {
    this._modal.open(CreateClientModal, { centered: true });
  }
}
