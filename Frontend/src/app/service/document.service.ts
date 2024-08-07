import { HttpClient, HttpParams } from "@angular/common/http";
import { ResponseMessage } from "../entity/responseMessage";
import { Injectable } from "@angular/core";
import { Observable, retry } from "rxjs";

@Injectable({
    providedIn:'root'
})
export class DocumentService{
    
    private apiUrl : string = "http://localhost:5001";

    documentId : string | null = null;
    constructor(private httpClient : HttpClient){

    }

    setDocumentId(docId : string | null){
        if(docId != null){
            this.documentId = docId;

        }
    }
    getDocumnetId() : string | null{
        return this.documentId;
    }
    uploadDocument(file : File) : Observable<ResponseMessage>{
        const formData = new FormData();
        formData.append('file',file)
        return this.httpClient.post<ResponseMessage>(`${this.apiUrl}/upload`,formData);
    }

    askQuestion(documentId : string , query : string): Observable<ResponseMessage> {
        const body ={documentId,query}
        return this.httpClient.post<ResponseMessage>(`${this.apiUrl}/ask-with-documentId`,body);
    }

    summarizeDoument(documentId: string ): Observable<ResponseMessage> {

        const query : string = 'summarize';
        const body ={documentId,query};
        return this.httpClient.post<ResponseMessage>(`${this.apiUrl}/summary-documentId`,body);
      }
}