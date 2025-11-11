export interface TagInfo {
  name: string
  size: number
  created: string
  digest?: string
}

export interface RepositoryInfo {
  name: string
  tagCount: number
  totalSize?: number
  lastPushed?: string
  tags?: TagInfo[]
}

export interface ManifestInfo {
  contentType: string
  digest?: string
  config: string
}

export interface ActivityLog {
  id: string
  userId: string
  action: string
  resource: string
  timestamp: string
  success: boolean
  details?: string
}

export interface User {
  id: string
  phoneNumber: string
  username?: string
  role: 'Admin' | 'User' | 'ReadOnly'
  createdAt: string
}
