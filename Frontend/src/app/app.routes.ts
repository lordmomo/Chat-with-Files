import {  Routes } from "@angular/router";
import { DocumentChatComponent } from "./document-chat/document-chat.component";
import { DocumentUploadComponent } from "./document-upload/document-upload.component";

export const routes :Routes = [
    { path: 'upload', component: DocumentUploadComponent },
    { path: 'chat', component: DocumentChatComponent},
    { path: '', redirectTo: '/upload', pathMatch: 'full' },
]