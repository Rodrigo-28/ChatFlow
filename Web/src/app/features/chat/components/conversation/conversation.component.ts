import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  input,
  OnChanges,
  OnInit,
  output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute } from '@angular/router';
import { CacheService } from '../../../../shared/services/cache.service';
import { Conversation } from '../../interfaces/conversation.interface';
import { SendMessage } from '../../interfaces/send-message.interface';

@Component({
  selector: 'app-conversation',
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
  ],
  templateUrl: './conversation.component.html',
  styleUrl: './conversation.component.scss',
})
export class ConversationComponent implements OnInit, AfterViewInit, OnChanges {
  @ViewChild('messagesContainer')
  messagesContainer?: ElementRef<HTMLDivElement>;
  senderId: string | null = null;
  // Inputs
  conversation = input<Conversation | null>(null);
  receiverName = input<string | null>(null);

  // Outputs
  sendNewMessage = output<SendMessage>();

  // Dependencies
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private readonly cacheService = inject(CacheService);

  // Component state
  formGroup: FormGroup;
  receiverId: string | null = null;

  constructor() {
    this.formGroup = this.fb.group({
      message: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.receiverId = this.route.snapshot.paramMap.get('id');
    this.cacheService.getItem<string>('senderId').subscribe({
      next: (senderId) => {
        this.senderId = senderId;
      },
    });
  }

  ngAfterViewInit(): void {
    this.scrollToBottom();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['conversation']) {
      this.scrollToBottom();
    }
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      if (this.messagesContainer) {
        this.messagesContainer.nativeElement.scrollTop =
          this.messagesContainer.nativeElement.scrollHeight;
      }
    }, 0);
  }

  sendMessage(form: FormGroup) {
    if (form.valid) {
      const content = form.value.message;
      const payload: SendMessage = {
        conversationId: this.conversation()?.id || '',
        receiverId: this.receiverId || '',
        content: content,
      };
      this.sendNewMessage.emit(payload);
      this.formGroup.reset();
      this.scrollToBottom();
    }
  }
}
