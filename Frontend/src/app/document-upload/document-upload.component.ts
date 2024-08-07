
import { Component, OnInit } from '@angular/core';
import { DocumentService } from '../service/document.service';
import { DataSharingService } from '../service/datasharing.service';

@Component({
  selector: 'app-document-upload',
  templateUrl: './document-upload.component.html',
  styleUrls: ['./document-upload.component.scss']
})
export class DocumentUploadComponent implements OnInit {

  selectedFile: File | null = null;
  documentId: string | null = null;
  uploadMessage: string = '';
  fileUploadSuccess: boolean = false;
  summarizedDocSuccess: boolean = false;
  summarizedContent: string = '';
  responseCaught: boolean = false;
  isUploading: boolean = false;
  isSummarizing: boolean = false;

  constructor(private documentService: DocumentService, private dataSharingService: DataSharingService) { }

  ngOnInit(): void {
    this.fileUploadSuccess = false;
  }

  onFileChange(event: any) {
    this.selectedFile = event.target.files[0];
  }

  uploadFile() {
    if (this.selectedFile != null) {
      this.isUploading = true; 
      this.documentService.uploadDocument(this.selectedFile).subscribe({
        next: (response) => {
          this.isUploading = false; 
          if (response.success) {
            console.log(response);
            this.documentId = response.message;
            this.documentService.setDocumentId(this.documentId);
            this.uploadMessage = 'File uploaded successfully';
            this.fileUploadSuccess = true;
          } else {
            this.uploadMessage = response.message;
          }
        },
        error: (err) => {
          this.isUploading = false; 
          this.uploadMessage = 'File upload failed';
        }
      });
    } else {
      this.isUploading = false; 
      alert("Document must be provided");
    }
  }

  onSummarizeFile() {
    if (this.documentId != null) {
      this.isSummarizing = true; 
      this.documentService.summarizeDoument(this.documentId).subscribe({
        next: (response) => {
          this.isSummarizing = false; 
          if (response.success) {
            this.summarizedDocSuccess = true;
            this.summarizedContent = response.message;
            this.dataSharingService.updateSummaryData({
              question: "Summary of the provided document", 
              answer: response.message
            });
          }
        },
        error: (err) => {
          this.isSummarizing = false; 
          alert("Summarization failed");
        }
      });
    } else {
      this.isSummarizing = false; 
      alert("Document should be provided. Document ID not found");
    }
  }
}
