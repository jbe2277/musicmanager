# Architecture

## Namespace (Layer) diagram

```mermaid
flowchart TB

  subgraph P["Presentation"]
    direction LR
    P_P["Properties"]
    P_CT["Controls"]
    P_C["Converters"]
    P_DD["DesignData"]
    P_S["Services"]
    P_V["Views"]
  end

  I["Interop"]

  subgraph A["Applications"]
    direction LR
    A_P["Properties"]    
    A_C["Controllers"]
    A_DM["DataModels"]
    A_S["Services"]
    A_VM["ViewModels"]
    A_V["Views"]
  end

  subgraph D["Domain"]
    direction LR
    D_MF["MusicFiles"]
    D_P["Playlists"]
    D_T["Transcoding"]
  end

  P --> I
  P --> A
  A --> D

  P_CT --> P_S
  P_C --> P_S  
  P_V --> P_CT
  P_V --> P_C  
  P_V --> P_S

  A_C --> A_DM
  A_C --> A_S
  A_C --> A_VM
  A_DM <==> A_S
  A_S --> A_V
  A_VM --> A_DM
  A_VM --> A_S
  A_VM --> A_V

  D_P --> D_MF
  D_T --> D_MF
```

## Dependency Rules

[config.nsdepcop](./config.nsdepcop)
