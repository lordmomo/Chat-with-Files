import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DocumentService } from '../service/document.service';
import { DataSharingService } from '../service/datasharing.service';

@Component({
  selector: 'app-document-chat',
  templateUrl: './document-chat.component.html',
  styleUrls: ['./document-chat.component.scss']
})
export class DocumentChatComponent implements OnInit {

  documentId: string | null = null;
  query: string = '';
  responseMessage: string = '';
  messages: { question: string, answer: string }[] = [];

  caughtResponse : boolean = false;

  @ViewChild('chatMessages') chatMessages!: ElementRef;

  
  constructor(private documentService : DocumentService, private dataSharingService: DataSharingService) { }

  ngOnInit(): void {
    this.dataSharingService.currentSummaryData.subscribe(data => {
      if (data) {
        this.messages.push(data); 
        console.log(data)
        this.scrollToBottom(); 
      }
    });
  }

  askQuestion() {
    this.documentId = this.documentService.getDocumnetId();

    if (this.documentId && this.query) {
      this.caughtResponse = true;
      this.documentService.askQuestion(this.documentId, this.query).subscribe({
        next : (response)=>{
          this.caughtResponse = false;

          if(response.success){
            this.responseMessage = response.message
            console.log(response);
            this.messages.push({ question: this.query, answer: response.message });

            console.log(this.responseMessage)
            this.query = '';
            this.scrollToBottom(); 
          }
          else{
            alert(response.message)
            console.error(response.message);

          }
        },
        error : (err)=>{
          this.caughtResponse = false;
          this.messages.push({ question: this.query, answer: 'Failed to get response.' });
          this.query = ''; 
          this.scrollToBottom();
          this.responseMessage = 'Failed to get response.';
        }
      });
    }
  }
  scrollToBottom(): void {
    setTimeout(() => {

      const chatMessagesElement = this.chatMessages.nativeElement;
      chatMessagesElement.scrollTop = chatMessagesElement.scrollHeight;
    }
    );
  }
}
