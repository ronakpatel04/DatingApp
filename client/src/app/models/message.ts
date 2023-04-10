export interface Message {
    id: number
    senderId: number
    senderUsername: string
    senderPhotoUrl: string
    receiverId: number
    receiverUsername: string
    receiverPhotoUrl: string
    content: string
    dateRead: string
    messageSent: string
  }