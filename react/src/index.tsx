import { render } from '@reactunity/renderer';
import './index.css';
import Chat from './chat/Chat'

function App() {
  return <view className="fixed top-0 bottom-0 left-0 right-0">
    <text>{`Go to <color=red>src/index.tsx</color> to edit this file`}</text>
    <Chat />
  </view>;
}

render(<App />);
