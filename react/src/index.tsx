import { render } from '@reactunity/renderer'
import './index.css'
import Chat from './player/Chat'
import { Auth } from './lobby/Auth'

function App() {
  return (
    <view className="fixed top-0 bottom-0 left-0 right-0 flex justify-center items-center">
      <Auth />
      <Chat />
    </view>
  )
}

render(<App />)
