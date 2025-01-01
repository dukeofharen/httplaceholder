export interface MenuItemModel {
  title: string
  icon: string
  routeName?: string
  url?: string
  targetBlank?: boolean
  hideWhenAuthEnabledAndNotLoggedIn?: boolean
  onlyShowWhenLoggedInAndAuthEnabled?: boolean
  onClick?: () => Promise<any>
}
